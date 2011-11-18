using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Nuxleus.Asynchronous;
using Nuxleus.Core.Performance;

namespace Nuxleus.Core.Net
{
    internal class FileOperations
    {
        public FileOperations() { }

        public static readonly FileOperations Instance = SingletonProvider<FileOperations>.Instance;
        private static readonly FileOperations @this = FileOperations.Instance;

        private static readonly FileOperationsConfiguration @fileOperationsConfiguration = SingletonProvider<FileOperationsConfiguration>.Instance;
        internal static FileOperationsConfiguration FileOperationsConfigurationInstance { get { return @fileOperationsConfiguration; } }
        

        private static readonly ConfigurationUtility @configUtil = ConfigurationUtility.Instance;
        private static readonly LoggerScope @scopeLog = new LoggerScope();
        private static readonly ExceptionHandlerScope @scopeExHandler = new ExceptionHandlerScope();
        private static readonly ProfilerScope @scopeProfiler = new ProfilerScope();

        static FileOperations()
        {
            InitializeConfiguration();
        }

        internal class FileOperationsConfiguration
        {

            private static readonly FileOperationsConfiguration @this = @fileOperationsConfiguration;

            private static readonly int m_baseWorkerQueues = Environment.ProcessorCount;
            private static readonly int m_workerQueueMultiplier = 1;
            private static readonly int m_workers = ((m_baseWorkerQueues) * (m_workerQueueMultiplier));
            private static readonly int m_maxFileDownloadRetries = 5;
            private static readonly bool m_loadBalanceOperations = false;

            internal int BaseWorkerQueues { get; set; }
            internal int WorkerQueueMultiplier { get; set; }
            internal int Workers { get; set; }
            internal int MaxFileDownloadRetries { get; set; }
            internal bool LoadBalanceOperations { get; set; }

            static FileOperationsConfiguration()
            {
                @this.BaseWorkerQueues = (ConfigurationUtility.GetIntFromAppSettings("BaseWorkerQueues", m_baseWorkerQueues));
                @this.WorkerQueueMultiplier = (ConfigurationUtility.GetIntFromAppSettings("BaseWorkerQueues", m_workerQueueMultiplier));
                @this.Workers = (ConfigurationUtility.GetIntFromAppSettings("Workers", m_workers));
                @this.MaxFileDownloadRetries = (ConfigurationUtility.GetIntFromAppSettings("MaxFileDownloadRetries", m_maxFileDownloadRetries));
                @this.LoadBalanceOperations = (ConfigurationUtility.GetBoolFromAppSettings("LoadBalanceOperations", m_loadBalanceOperations));
            }

        }

        private static void InitializeConfiguration()
        {
            ConfigurationUtility.ServicePointManagerConfiguration @servicePointManagerConfig = ConfigurationUtility.ServicePointManagerConfigurationInstance;
            ConfigurationUtility.ThreadPoolConfiguration @threadPoolConfig = ConfigurationUtility.ThreadPoolConfigurationInstance;
            ConfigurationUtility.HttpRuntimeConfiguration @httpRuntimeConfig = ConfigurationUtility.HttpRuntimeConfigurationInstance;

            @this.LogInfo("system.web/httpRuntime/minFreeThreads: {0}", ConfigurationUtility.HttpRuntimeSection.MinFreeThreads);
            @this.LogInfo("system.web/httpRuntime/minLocalRequestFreeThreads: {0}", ConfigurationUtility.HttpRuntimeSection.MinLocalRequestFreeThreads);

            @this.LogInfo("ServicePointManager Default Connection Limit: {0}", ServicePointManager.DefaultConnectionLimit);
            @this.LogInfo("ServicePointManager Maximum Service Points: {0}", ServicePointManager.MaxServicePoints);
            ServicePointManager.DefaultConnectionLimit = @servicePointManagerConfig.DefaultConnectionLimit;
            ServicePointManager.EnableDnsRoundRobin = @servicePointManagerConfig.EnableDnsRoundRobin;
            ServicePointManager.MaxServicePoints = @servicePointManagerConfig.MaxServicePoints;
            ServicePointManager.MaxServicePointIdleTime = @servicePointManagerConfig.MaxServicePointIdleTime;
            ServicePointManager.ServerCertificateValidationCallback = @servicePointManagerConfig.ServerCertificateValidationCallback;
            @this.LogInfo("ServicePointManager Default Connection Limit After App Configuration: {0}", ServicePointManager.DefaultConnectionLimit);
            @this.LogInfo("ServicePointManager Maximum Service Points After App Configuration: {0}", ServicePointManager.MaxServicePoints);

            ThreadPool.SetMaxThreads(@threadPoolConfig.MaxiumWorkerThreads, @threadPoolConfig.MaximumAsyncIOThreads);
            ThreadPool.SetMinThreads(@threadPoolConfig.MinimumWorkerThreads, @threadPoolConfig.MinimumAsyncIOThreads);
        }



        public static void Invoke<T>(IEnumerable<IEnumerable<IAsync>> operations)
        {
            FileOperationsConfiguration @fileOperationsConfig = FileOperationsConfigurationInstance;
            int workers = @fileOperationsConfig.Workers;
            bool loadBalanceOperations = @fileOperationsConfig.LoadBalanceOperations;

            Scope scope = new Scope();
            scope += scopeProfiler.Scope;
            scope += @scopeLog.Scope;
            scope += @scopeExHandler.Scope;

            @scopeLog.Message = "Begin Processing File Transfer Requests";

            scope.Begin = () =>
            {
                if (loadBalanceOperations)
                {
                    int totalTasksEnqueued = 0;
                    using (IEnumerableTWorkerQueue<IAsync> q = new IEnumerableTWorkerQueue<IAsync>(workers))
                    {
                        foreach (IEnumerable<IAsync> operation in operations)
                        {
                            q.EnqueueTask(operation);
                            totalTasksEnqueued += 1;
                        }
                        @this.LogInfo("Total Tasks Enqueued: {0}, Total Queues: {1}", totalTasksEnqueued, workers);
                    }
                }
                else
                {
                    InvokeAsyncParallelOperation(operations).Execute();
                }

                @this.LogInfo("Total Processing Time: {0}", scopeProfiler.EllapsedTime.Milliseconds);
            };
        }

        static IEnumerable<IAsync> InvokeAsyncParallelOperation(IEnumerable<IEnumerable<IAsync>> tasks)
        {
            yield return Async.Parallel(tasks.ToArray());
            Parallel.Invoke(() =>
            {
                Console.WriteLine("Begin first task...");
            });
        }

        static void InvokeAsyncParallelOperation(IEnumerable<Task> tasks)
        {
            //Parallel.ForEach(tasks, task => Process(task)); 
            // Parallel.Invoke(tasks.ToArray());
            // {
            //     Console.WriteLine("Begin first task...");
            //     //GetLongestWord(words);
            // },  // close first Action

            //                  () =>
            //                  {
            //                      Console.WriteLine("Begin second task...");
            //                      //GetMostCommonWords(words);
            //                  }, //close second Action

            //                  () =>
            //                  {
            //                      Console.WriteLine("Begin third task...");
            //                      //GetCountForWord(words, "species");
            //                  } //close third Action
            // ); //close parallel.invoke
        }

        //static IEnumerable<IEnumerable<IAsync>> InvokeOperationAsync(IEnumerable<IAsync> operation)
        //{
        //    return from fileInfo in operation
        //           select CreateCopyFileTask(fileInfo).AsIAsync();
        //}

        //public static CopyFileTask CreateCopyFileTask(FileCopyInfo fileInfo)
        //{
        //    return new CopyFileTask { HttpLocation = fileInfo.HttpLocation, DestinationServer = fileInfo.DestinationServer, FileName = fileInfo.NewFileName };
        //}

        public PerformanceLog InvokeCopyFileTask(ITask copyFileTask)
        {
            Stopwatch timer = new Stopwatch();
            Stopwatch.UnitPrecision = UnitPrecision.NANOSECONDS;
            PerformanceLog perfLog = new PerformanceLog
            {
                Entries = new List<Entry>(),
                UnitPrecision = Stopwatch.UnitPrecision
            };

            Uri uri = new Uri(String.Format("http://localhost:9999/Person_{0}.xml", 1), UriKind.Absolute);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);


            using (timer)
            {

                timer.Scope = () =>
                {

                    timer.LogScope("Create a Person object", perfLog, PerformanceLogEntryType.CompiledObjectCreation, () =>
                    {
                        //person = CreatePerson(sequenceID);
                    });

                    Stream serializedMemoryStream = null;

                    timer.LogScope("Serialize the Person object to a MemoryStream", perfLog, PerformanceLogEntryType.Serialization,
                        () =>
                        {
                            //serializedMemoryStream = SerializeToStream<Person>(person, null, agent.ISerializerTestAgent);
                        }).LogData("Length (in bytes) of memoryStream", serializedMemoryStream.Length, PerformanceLogEntryType.StreamSize);


                    timer.LogScope("Send the serialized MemoryStream to S3", perfLog, PerformanceLogEntryType.SendSerializedObjectTime, () =>
                    {
                        //client.OpenWriteAsync(uri);
                    });

                    timer.LogScope("Request the serialized object back from S3", perfLog, PerformanceLogEntryType.ReceiveSerializedObjectTime, () =>
                    {
                        //request.BeginGetResponse(new AsyncCallback(ReadCallback), request);
                    });

                    using (serializedMemoryStream)
                    {
                        timer.LogScope("Deserialize and parse the Person object from a MemoryStream", perfLog, PerformanceLogEntryType.Deserialization, () =>
                        {
                            //newPersonFromMemoryStream = DeserializeFromStream<Person>(serializedMemoryStream, agent.ISerializerTestAgent);
                        });
                    }

                    //CompareValuesAndLogResults(person, newPersonFromMemoryStream, perfLog, typeof(MemoryStream), PerformanceLogEntryType.DeserializationCorrect);

                };

                perfLog.LogData("Duration of test", timer.Duration, PerformanceLogEntryType.TotalDuration);
            }

            return perfLog;
        }

        //void ReadCallback(IAsyncResult asyncResult)
        //{

        //    HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
        //    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);

        //    using (Stream stream = response.GetResponseStream())
        //    {


        //    }
        //}

        //static void AnalyzeValuesAndLogResults(Person person, Person newPerson, PerformanceLog perfLog, Type streamType, PerformanceLogEntryType type)
        //{
        //    perfLog.LogData(String.Format("newPersonFrom{0}.Name and person.Name are equal", streamType.Name), String.Equals(newPerson.Name, person.Name), type);
        //    perfLog.LogData(String.Format("newPersonFrom{0}.ID and person.ID are equal", streamType.Name), int.Equals(newPerson.ID, person.ID), type);
        //    perfLog.LogData(String.Format("newPersonFrom{0}.Email and person.Email are equal", streamType.Name), String.Equals(newPerson.Email, person.Email), type);

        //    PhoneNumber[] phone = person.Phone.ToArray();
        //    PhoneNumber[] newPhone = newPerson.Phone.ToArray();

        //    for (int i = 0; i < phone.Length; i++)
        //    {
        //        perfLog.LogData(String.Format("PhoneNumber[{0}].Number from newPersonFrom{1}.Phone is the same as PhoneNumber[{0}].Number from person{1}.Phone", i, streamType.Name), phone[i].Number.Equals(newPhone[i].Number), type);
        //        perfLog.LogData(String.Format("PhoneNumber[{0}].Type from newPersonFrom{1}.Phone is the same as PhoneNumber[{0}].Type from person{1}.Phone", i, streamType.Name), phone[i].Type.Equals(newPhone[i].Type), type);
        //    }
        //}
        //public static CopyFilesResponse GetFilesOverHttp(CopyFilesRequest request)
        //{



        //    //Task<Stream>.Factory.StartNew(() =>
        //    //{
        //    //    //create a web request to get the original file
        //    //    WebRequest webRequest = WebRequest.Create(request.HttpLocation);
        //    //    //request.Timeout = 60000;
        //    //    using (Stream responseStream = request.GetResponse().GetResponseStream())
        //    //    {
        //    //        //create a filestream to write to
        //    //        string filePath = file.DestinationServer + file.NewFileName;
        //    //        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        //    //        {
        //    //            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        //    //        }

        //    //    }
        //    //}).ContinueWith(task => Console.WriteLine(task.Result));

        //    //using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        //    //{
        //    //    //use a buffer to copy from the web request stream to the file stream
        //    //    byte[] buffer = new byte[262144];
        //    //    int readCount;
        //    //    while ((readCount = responseStream.Read(buffer, 0, buffer.Length)) != 0)
        //    //    {
        //    //        fileStream.Write(buffer, 0, readCount);
        //    //    }
        //    //}


        //    CopyFilesResponse response = new CopyFilesResponse();

        //    try
        //    {

        //        foreach (FileCopyInfo file in request.FilesToCopy)
        //        {

        //        }

        //        response.FilesCopied = new List<FileCopyResult>();

        //        //copy the file copy results
        //        foreach (FileCopyInfo file in request.FilesToCopy)
        //        {
        //            FileCopyResult result = new FileCopyResult()
        //            {
        //                FileName = file.NewFileName,
        //                SuccessfullyCopied = file.SuccessfullyCopied,
        //                ErrorMessage = file.ErrorMessage
        //            };
        //            response.FilesCopied.Add(result);
        //            file.FileCopyCompleteSignal.Close();
        //        }
        //        response.Successful = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Successful = false;
        //        response.ErrorMessage = ex.Message;

        //    }
        //    return response;
        //}

        //private static void WorkerThread()
        //{
        //    FileCopyInfo file = new FileCopyInfo();
        //    try
        //    {
        //        int tryCount = 0;
        //        bool copied = false;
        //        string errorMessage = String.Empty;
        //        while (tryCount < 5)
        //        {
        //            try
        //            {
        //                tryCount++;
        //                //create a web request to get the original file
        //                WebRequest request = WebRequest.Create(file.HttpLocation);
        //                //request.Timeout = 60000;
        //                using (Stream responseStream = request.GetResponse().GetResponseStream())
        //                {
        //                    //create a filestream to write to
        //                    string filePath = file.DestinationServer + file.NewFileName;
        //                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        //                    {
        //                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        //                    }
        //                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        //                    {
        //                        //use a buffer to copy from the web request stream to the file stream
        //                        byte[] buffer = new byte[262144];
        //                        int readCount;
        //                        while ((readCount = responseStream.Read(buffer, 0, buffer.Length)) != 0)
        //                        {
        //                            fileStream.Write(buffer, 0, readCount);
        //                        }
        //                    }
        //                }
        //                copied = true;

        //                //break out of the loop
        //                break;
        //            }
        //            catch (Exception ex)
        //            {
        //                copied = false;
        //                errorMessage = ex.Message;
        //            }
        //        }
        //        if (copied)
        //        {
        //            file.SuccessfullyCopied = true;
        //            file.FileCopyCompleteSignal.Set();
        //        }
        //        else
        //        {
        //            file.SuccessfullyCopied = false;
        //            file.ErrorMessage = errorMessage;
        //            file.FileCopyCompleteSignal.Set();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (file != null)
        //        {
        //            file.SuccessfullyCopied = false;
        //            file.ErrorMessage = ex.Message;
        //            file.FileCopyCompleteSignal.Set();
        //        }
        //    }
        //}
    }
}
