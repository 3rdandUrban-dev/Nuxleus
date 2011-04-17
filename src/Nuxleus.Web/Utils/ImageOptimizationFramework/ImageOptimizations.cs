/*
 * Originally published as part of the "Sprite and Image Optimization Framework Preview 2" found at http://aspnet.codeplex.com/releases/view/50869
 * as part of the ASP.NET Open Source project on CodePlex.com (http://aspnet.codeplex.com).
 * As specified at http://aspnet.codeplex.com/license the code contained in the above preview release is licensed under
 * a Microsoft Source License for ASP.NET Pre-Release Components, a copy of which can be found at ~/license/MicrosoftSourceLicense-for-ASP.NET-Pre-ReleaseComponents.tx
 * in this projects directory structure.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace Nuxleus.Web.Utils {

    /// <summary>
    /// Automates the creation of sprites and base64 inlining for CSS
    /// </summary>
    public static class ImageOptimizations {
        private static readonly string[] _extensions = { "*.jpg", "*.gif", "*.png", "*.bmp", "*.jpeg" };
        private static readonly object _lockObj = new object();
        public const string TimestampFileName = "timeStamp.dat";
        public const string SettingsFileName = "settings.xml";
        public const string HighCompatibilityCssFile = "highCompat.css";
        public const string LowCompatibilityCssFile = "lowCompat.css";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Inlined")]
        public const string InlinedTransparentGif = "data:image/gif;base64,R0lGODlhAQABAIABAP///wAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";

        /// <summary>
        /// Rebuilds the cache / dependancies for all subfolders below the specified directory
        /// </summary>
        /// <param name="path">The root directory for the cache rebuild (usually app_sprites)</param>
        /// <param name="rebuildImages">Indicate whether the directories should be rebuilt as well</param>
        public static void AddCacheDependencies(string path, bool rebuildImages) {

            string spriteFolderRelativePath = (string)HttpRuntime.Cache.Get("spriteFolderRelativePath");
            List<string> subDirectories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories).ToList();
            subDirectories.Add(path);

            foreach (string subDirectory in subDirectories) {

                if (rebuildImages) {
                    ProcessDirectory(subDirectory, spriteFolderRelativePath, true);
                }

                InsertItemIntoCache(subDirectory, Directory.GetDirectories(subDirectory));
            }

            return;
        }

        /// <summary>
        /// Called when the cache dependancy of a subdirectory of the root image folder is modified, created, or removed
        /// </summary>
        private static void RebuildFromCacheHit(string key, object value, CacheItemRemovedReason reason) {

            var data = (Tuple<string, IEnumerable<string>>)value;
            string path = data.Item1;
            string spriteFolderRelativePath = (string)HttpRuntime.Cache.Get("spriteFolderRelativePath");
            //string spriteFolderRelativePath = 
            IEnumerable<string> cachedDirectoriesBelowCurrentFolder = data.Item2;
            IEnumerable<string> directoriesBelowCurrentFolder;

            try {
                directoriesBelowCurrentFolder = Directory.GetDirectories(path);
            }
            // If the directory is not found, it was probably deleted, and the cache item does not need to be re-inserted
            catch (Exception) {
                if (!Directory.Exists(path)) {
                    return;
                }
                throw;
            }

            switch (reason) {
                case CacheItemRemovedReason.DependencyChanged:
                    if (ProcessDirectory(path, spriteFolderRelativePath, true)) {
                        // Add new directories to the cache                      
                        if (!directoriesBelowCurrentFolder.SequenceEqual(cachedDirectoriesBelowCurrentFolder)) {
                            foreach (string directory in directoriesBelowCurrentFolder.Except(cachedDirectoriesBelowCurrentFolder)) {
                                AddCacheDependencies(directory, true);
                            }
                        }

                        // Add the current directory back into the cache
                        InsertItemIntoCache(path, directoriesBelowCurrentFolder);

                        // Rebuild subdirectories without a settings file if they inherit from this directory
                        if (File.Exists(Path.Combine(path, SettingsFileName))) {
                            foreach (string subFolder in directoriesBelowCurrentFolder) {
                                if (!File.Exists(Path.Combine(subFolder, SettingsFileName))) {
                                    HttpRuntime.Cache.Remove(subFolder);
                                }
                            }
                        }
                    }
                    break;

                // Cache items will only be manually removed if they have to be rebuilt due to changes in a folder that they inherit settings from
                case CacheItemRemovedReason.Removed:
                    if (ProcessDirectory(path, spriteFolderRelativePath, false)) {
                        InsertItemIntoCache(path, directoriesBelowCurrentFolder);

                        foreach (string subFolder in directoriesBelowCurrentFolder) {
                            if (!File.Exists(Path.Combine(subFolder, SettingsFileName))) {
                                HttpRuntime.Cache.Remove(subFolder);
                            }
                        }
                    }
                    break;

                case CacheItemRemovedReason.Expired:
                case CacheItemRemovedReason.Underused:
                    // Don't need to reprocess parameters, just re-insert the item into the cache
                    HttpRuntime.Cache.Insert(key, value, new CacheDependency(path), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, RebuildFromCacheHit);
                    break;

                default:
                    break;
            }
            return;
        }

        private static void InsertItemIntoCache(string path, IEnumerable<string> directoriesBelowCurrentFolder) {
            string key = Guid.NewGuid().ToString();
            var value = Tuple.Create(path, directoriesBelowCurrentFolder);
            HttpRuntime.Cache.Insert(key, value, new CacheDependency(path), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, RebuildFromCacheHit);
        }

        /// <summary>
        /// Executes the image optimizer on a specific subdirectory of the root image folder (non-recursive)
        /// </summary>
        /// <param name="path">The path to the directory to be rebuilt</param>
        /// <returns>False if the directory does not exist</returns>
        /// <param name="checkIfFilesWereModified">Indicate whether the directory should only be rebuilt if files were modified</param>
        public static bool ProcessDirectory(string path, string spriteFolderRelativePath, bool checkIfFilesWereModified) {
            // Check if directory was deleted
            if (!Directory.Exists(path))
                return false;

            try {
                if (checkIfFilesWereModified && !HaveFilesBeenModified(path)) {
                    return true;
                }

                // Make a list of the disk locations of each image
                List<string> imageLocations = new List<string>();

                foreach (string extension in _extensions) {
                    imageLocations.AddRange(Directory.GetFiles(path, extension));
                }

                // Make sure to delete any existing sprites (or other images with the filename sprite###.imageExtension)
                imageLocations.RemoveAll(DeleteSpriteFile);

                // Import settings from settings file
                ImageSettings settings = ReadSettings(path, spriteFolderRelativePath);

                // Create pointer to the CSS output file
                lock (_lockObj) {
                    using (TextWriter cssHighCompatOutput = new StreamWriter(Path.Combine(path, HighCompatibilityCssFile), append: false),
                                      cssLowCompatOutput = new StreamWriter(Path.Combine(path, LowCompatibilityCssFile), append: false)) {

                        PerformOptimizations(path, spriteFolderRelativePath, settings, cssHighCompatOutput, cssLowCompatOutput, imageLocations);

                        // Merge with a user's existing CSS file(s)
                        MergeExternalCss(path, cssHighCompatOutput, cssLowCompatOutput);
                        
                        // TODO: CSS minification call should occur here
                    }
                }

                imageLocations.Clear();
                foreach (string extension in _extensions) {
                    imageLocations.AddRange(Directory.GetFiles(path, extension));
                }

                SaveFileModificationData(path);
                return true;
            }
            catch (Exception) {
                if (!Directory.Exists(path)) {
                    return false;
                }
                throw;
            }
        }

        private static void MergeExternalCss(string path, TextWriter cssHighCompatOutput, TextWriter cssLowCompatOutput) {
            string[] extraCssFiles = Directory.GetFiles(path, "*.css");

            foreach (string cssFile in extraCssFiles) {
                if (cssFile.Contains(HighCompatibilityCssFile) || cssFile.Contains(LowCompatibilityCssFile)) {
                    continue;
                }

                using (TextReader cssRead = new StreamReader(cssFile)) {
                    string textToBeCopied = cssRead.ReadToEnd();

                    cssHighCompatOutput.Write(textToBeCopied);
                    cssLowCompatOutput.Write(textToBeCopied);
                }
            }
        }

        private static void SaveFileModificationData(string path) {
            using (TextWriter timeStamp = new StreamWriter(GetTimeStampFile(path))) {
                timeStamp.Write(GetCurrentTimeStampInfo(path));
            }
        }

        /// <summary>
        /// Reads the timestamps of all of the files within a directory, and outputs them in a single sorted string. Used to determine if changes have occured to a directory upon application start.
        /// </summary>
        /// <param name="path">The path to the directory</param>
        /// <returns>A sorted string containing all filenames and last modified timestamps</returns>
        private static string GetCurrentTimeStampInfo(string path) {
            List<string> fileLocations = Directory.GetFiles(path).ToList();

            // Remove the timestamp file, since it can't be included in the comparison
            string timeStampFile = GetTimeStampFile(path);
            fileLocations.Remove(timeStampFile);
            fileLocations.Sort();

            StringBuilder timeStampBuilder = new StringBuilder();

            foreach (string file in fileLocations) {
                string name = Path.GetFileName(file);
                DateTime lastModificationTime = File.GetLastWriteTimeUtc(file);

                timeStampBuilder.Append(name).Append(lastModificationTime);
            }

            return timeStampBuilder.ToString();
        }

        private static string GetSavedTimeStampInfo(string path) {
            try {
                using (TextReader timeStamp = new StreamReader(GetTimeStampFile(path))) {
                    return timeStamp.ReadToEnd();
                }
            }
            // In the case of an exception, regenerate all sprites
            catch (FileNotFoundException) {
                return null;
            }
        }

        private static string GetTimeStampFile(string path) {
            return Path.Combine(path, TimestampFileName);
        }

        private static bool HaveFilesBeenModified(string path) {
            return GetCurrentTimeStampInfo(path) != GetSavedTimeStampInfo(path);
        }

        /// <summary>
        /// Checks if an image (passed by path or image name) is a sprite image or CSS file created by the framework
        /// </summary>
        /// <param name="path">The path or filename of the image in question</param>
        /// <returns>True if the image is a sprite, false if it is not</returns>
        public static bool IsOutputSprite(string path) {
            string name = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path).TrimStart('.');
            List<string> imageExtensions = new List<string>(_extensions);

            return ((Regex.Match(name, @"^sprite[0-9]*$").Success && imageExtensions.Contains("*." + extension)) || extension == "css");
        }

        /// <summary>
        /// Checks if the image at the path is a sprite generated by the framework, and deletes it if it was
        /// </summary>
        /// <param name="path">The file path to the image in question</param>
        /// <returns>True if the image was a sprite (and was by extension, deleted)</returns>
        private static bool DeleteSpriteFile(string path) {
            if (IsOutputSprite(path)) {
                File.Delete(path);
                return true;
            }
            return false;
        }

        private static void PerformOptimizations(string path, string spriteFolderRelativePath, ImageSettings settings, TextWriter cssHighCompatOutput, TextWriter cssLowCompatOutput, List<string> imageLocations) {
            // Create a list containing each image (in Bitmap format), and calculate the total size (in pixels) of final image        
            int x = 0;
            int y = 0;
            int imageIndex = 0;
            long size = 0;
            int spriteNumber = 0;
            List<Bitmap> images = new List<Bitmap>();

            try {
                foreach (string imagePath in imageLocations) {
                    // If the image is growing above the specified max file size, make the sprite with the existing images
                    // and add the new image to the next sprite list

                    if ((imageIndex > 0) && IsSpriteOversized(settings.MaxSize, size, imagePath)) {
                        GenerateSprite(path, settings, x, y, spriteNumber, images, cssHighCompatOutput, cssLowCompatOutput);

                        // Clear the existing images
                        foreach (Bitmap image in images) {
                            image.Dispose();
                        }

                        // Reset variables to initial values, and increment the spriteNumber
                        images.Clear();
                        x = 0;
                        y = 0;
                        imageIndex = 0;
                        size = 0;
                        spriteNumber++;
                    }

                    // Add the current image to the list of images that are to be processed
                    images.Add(new Bitmap(imagePath));

                    // Use the image tag to store its name
                    images[imageIndex].Tag = MakeCssClassName(imagePath, spriteFolderRelativePath);

                    // Find the total pixel size of the sprite based on the tiling direction
                    if (settings.TileInYAxis) {
                        y += images[imageIndex].Height;
                        if (x < images[imageIndex].Width) {
                            x = images[imageIndex].Width;
                        }
                    }
                    else {
                        x += images[imageIndex].Width;
                        if (y < images[imageIndex].Height) {
                            y = images[imageIndex].Height;
                        }
                    }

                    // Update the filesize size of the bitmap list
                    size += (new FileInfo(imagePath)).Length;

                    imageIndex++;
                }

                // Merge the final list of bitmaps into a sprite
                if (imageIndex != 0)
                    GenerateSprite(path, settings, x, y, spriteNumber, images, cssHighCompatOutput, cssLowCompatOutput);
            }
        finally
                // Close the CSS file and clear the images list
            {
                foreach (Bitmap image in images) {
                    image.Dispose();
                }
                images.Clear();
            }
        }

        private static bool IsSpriteOversized(int maxSize, long spriteSize, string imagePath) {            
            // Estimate the size of the sprite after adding the current image
            long nextSize = spriteSize + new FileInfo(imagePath).Length;

            // Determine of the size is too large
            return nextSize > (1024 * maxSize);
        }

        /// <summary>
        /// Make the appropriate CSS ID name for the sprite to be used
        /// </summary>
        /// <param name="pathToImage">The path to the image</param>
        /// <param name="pathToSpriteFolder">The path to the folder used to store sprites, used if the path to the image was not relative to the sprites folder</param>
        /// <returns>The CSS class used to reference the optimized image</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static string MakeCssClassName(string pathToImage, string pathToSpriteFolder = null) {
            if (pathToSpriteFolder != null) {
                pathToImage = MakePathRelative(pathToImage, pathToSpriteFolder);
            }
            else {
                pathToImage = pathToImage.TrimStart('~').Trim('/');
            }
            return pathToImage.Replace('\\', '_').Replace('/', '_').Replace('.', '-');
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToUpper")]
        private static string MakePathRelative(string fullPath, string pathToRelativeRoot) {
            fullPath = fullPath.TrimStart('~').Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim(Path.AltDirectorySeparatorChar);
            pathToRelativeRoot = pathToRelativeRoot.TrimStart('~').Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim(Path.AltDirectorySeparatorChar);
            
            if (fullPath.ToUpper().Contains(pathToRelativeRoot.ToUpper())) {
                return fullPath.Remove(0, fullPath.IndexOf(pathToRelativeRoot, StringComparison.OrdinalIgnoreCase) + pathToRelativeRoot.Length + 1);
            }
            else {
            //    throw new ArgumentException("The path passed is not a subdirectory of the root image folder");
                return fullPath;
            }
        }

        /// <summary>
        /// Returns the name of the CSS file containing the best compatibility settings for the user's browser. Returns null if the browser does not support any optimizations. 
        /// </summary>
        /// <param name="browser">The HttpBrowserCapabilities object for the user's browser</param>
        /// <returns>The name of the correct CSS file, or Null if not supported</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static string LinkCompatibleCssFile(HttpBrowserCapabilitiesBase browser) {
            if (browser.Type.ToUpper().Contains("IE")) {
                if (browser.MajorVersion <= 7) {
                    return null;
                }
                else if (browser.MajorVersion < 9) {
                    return LowCompatibilityCssFile;
                }
            }
            else if (browser.Type.ToUpper().Contains("FIREFOX")) {
                if ((browser.MajorVersion <= 3) && (browser.MinorVersion < 5)) {
                    return LowCompatibilityCssFile;
                }
            }
            return HighCompatibilityCssFile;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void GenerateSprite(string path, ImageSettings settings, int x, int y, int spriteNumber, List<Bitmap> images, TextWriter cssHighCompatOutput, TextWriter cssLowCompatOutput) {
            // Create a drawing surface and add the images to it
            using (Bitmap sprite = new Bitmap(x, y))
            using (Graphics drawingSurface = Graphics.FromImage(sprite)) {

                // Set the background to the specs from the settings file
                drawingSurface.Clear(settings.BackgroundColor);

                // Make the final sprite and save it
                int xOffset = 0;
                int yOffset = 0;
                foreach (Bitmap image in images) {
                    drawingSurface.DrawImage(image, new Rectangle(xOffset, yOffset, image.Width, image.Height));

                    // Add the CSS data
                    GenerateCss(xOffset, yOffset, spriteNumber, settings.Format, settings.Base64, image, cssHighCompatOutput);
                    GenerateCss(xOffset, yOffset, spriteNumber, settings.Format, false, image, cssLowCompatOutput);

                    if (settings.TileInYAxis) {
                        yOffset += image.Height;
                    }
                    else {
                        xOffset += image.Width;
                    }
                }

                // Set the encoder parameters and make the image
                try {
                    using (EncoderParameters spriteEncoderParameters = new EncoderParameters(1)) {
                        spriteEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, settings.Quality);

                        // Attempt to save the image to disk with the specified encoder
                        sprite.Save(Path.Combine(path, "sprite" + spriteNumber + "." + settings.Format), GetEncoderInfo(settings.Format), spriteEncoderParameters);
                    }
                }
                catch (Exception) {
                    // If errors occur, get the CLI to auto-choose an encoder. Unfortunately this means that the quality settings will be not used.
                    try {
                        sprite.Save(Path.Combine(path, "sprite" + spriteNumber + "." + settings.Format));
                    }
                    catch (Exception) {
                        // If errors occur again, try to save as a png
                        sprite.Save(Path.Combine(path, "sprite" + spriteNumber + ".png"));
                    }
                }
            }
            return;
        }

        private static void GenerateCss(int xOffset, int yOffset, int spriteNumber, string fileExtension, bool base64, Bitmap image, TextWriter cssOutput) {
            cssOutput.WriteLine("." + (string)image.Tag);
            cssOutput.WriteLine("{");
            cssOutput.WriteLine("width:" + image.Width + "px;");
            cssOutput.WriteLine("height:" + image.Height + "px;");

            switch (base64) {
                case true:
                    cssOutput.WriteLine("background: url(data:image/" + fileExtension + ";base64," + ConvertImageToBase64(image, GetImageFormat(fileExtension)) + ") no-repeat 0% 0%;");
                    break;

                default:
                    cssOutput.WriteLine("background-image:url(sprite" + spriteNumber + "." + fileExtension + ");");
                    cssOutput.WriteLine("background-position:-" + xOffset + "px -" + yOffset + "px;");
                    break;
            }

            cssOutput.WriteLine("}");
            return;
        }

        private static string ConvertImageToBase64(Bitmap image, ImageFormat format) {
            string base64;
            using (MemoryStream memory = new MemoryStream()) {
                image.Save(memory, format);
                base64 = Convert.ToBase64String(memory.ToArray());
            }
            return base64;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToUpper")]
        private static ImageFormat GetImageFormat(string fileExtension) {
            switch (fileExtension.ToUpper()) {
                case "JPG":
                case "JPEG":
                    return ImageFormat.Jpeg;

                case "GIF":
                    return ImageFormat.Gif;

                case "PNG":
                    return ImageFormat.Png;

                case "BMP":
                    return ImageFormat.Bmp;

                default:
                    return ImageFormat.Png;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String,System.Globalization.NumberStyles)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static ImageSettings ReadSettings(string path, string spriteFolderRelativePath) {
            ImageSettings settings = new ImageSettings();
            XmlTextReader settingsData;

            // Open the settings file. If it cannot be opened, or one cannot be found, use defaults
            try {
                using (settingsData = new XmlTextReader(Path.Combine(path, SettingsFileName))) {
                    while (settingsData.Read()) {
                        if (settingsData.NodeType == XmlNodeType.Element) {
                            string nodeName = settingsData.Name;

                            if (nodeName.Equals("FileFormat", StringComparison.OrdinalIgnoreCase)) {
                                settings.Format = settingsData.ReadElementContentAsString().Trim('.');
                            }
                            else if (nodeName.Equals("Quality", StringComparison.OrdinalIgnoreCase)) {
                                settings.Quality = settingsData.ReadElementContentAsInt();
                            }
                            else if (nodeName.Equals("MaxSize", StringComparison.OrdinalIgnoreCase)) {
                                settings.MaxSize = settingsData.ReadElementContentAsInt();
                            }
                            else if (nodeName.Equals("BackgroundColor", StringComparison.OrdinalIgnoreCase)) {
                                string output = settingsData.ReadElementContentAsString();
                                int temp = Int32.Parse(output, System.Globalization.NumberStyles.HexNumber);
                                settings.BackgroundColor = Color.FromArgb(temp);
                            }
                            else if (nodeName.Equals("Base64Encoding", StringComparison.OrdinalIgnoreCase)) {
                                settings.Base64 = settingsData.ReadElementContentAsBoolean();
                            }
                            else if (nodeName.Equals("TileInYAxis", StringComparison.OrdinalIgnoreCase)) {
                                settings.TileInYAxis = settingsData.ReadElementContentAsBoolean();
                            }
                        }
                    }
                }
                return settings;

            }
            catch (FileNotFoundException) {
                // If no file is found, recursively check parent directories up until the root image folder for a settings file
                if (path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar).EndsWith(spriteFolderRelativePath.TrimStart('~').Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim(Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)) {
                    // If the current directory is the root image folder, use the default values
                    return settings;
                }
                try {
                    return ReadSettings(Directory.GetParent(path).FullName, spriteFolderRelativePath);
                }
                catch (Exception) {
                    return settings;
                }
            }
            // If any other exceptions occur, use the default values
            catch (Exception) {
                return settings;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        private static ImageCodecInfo GetEncoderInfo(string format) {
            // Find the appropriate codec for the specified file extension
            if (format == "jpg")
                format = "jpeg";
            format = "image/" + format.ToLower();
            // Get a list of all the available encoders
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            // Search the list for the proper encoder
            foreach (ImageCodecInfo encoder in encoders) {
                if (encoder.MimeType == format)
                    return encoder;
            }

            // If a format cannot be found, throw an exception
            throw new FormatException("Encoder not found! The CLI will attempt to automatically choose an encoder, however image quality settings will be ignored");
        }
    }
}