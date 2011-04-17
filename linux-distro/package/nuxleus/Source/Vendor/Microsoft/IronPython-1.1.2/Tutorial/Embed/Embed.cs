/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public
 * License. A  copy of the license can be found in the License.html file at the
 * root of this distribution. If  you cannot locate the  Microsoft Public
 * License, please send an email to  dlr@microsoft.com. By using this source
 * code in any fashion, you are agreeing to be bound by the terms of the 
 * Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;
using WinForms = System.Windows.Forms;
using IronPython.Hosting;

public class EmbedApp : Application {
    public Canvas canvas;
    public Window mainWindow;

    private void RunScript(string name) {
        MessageBox.Show("add code here to run " + name);
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        CreateAndShowMainWindow();
    }

    private void CreateAndShowMainWindow() {
        // Create the application's main window
        mainWindow = new Window();

        // Create a canvas sized to fill the window
        canvas = new Canvas();
        canvas.Background = Brushes.LightSteelBlue;

        // Add a "Hello World!" text element to the Canvas
        TextBlock txt = new TextBlock();
        txt.FontSize = 30;
        txt.Text = "Hello World!";
        Canvas.SetTop(txt, 100);
        Canvas.SetLeft(txt, 100);
        canvas.Children.Add(txt);

        Button btn = new Button();
        btn.FontSize = 30;
        btn.Content = "Run Script";
        btn.Click += new RoutedEventHandler(btn_Click);
        Canvas.SetTop(btn, 20);
        Canvas.SetLeft(btn, 100);
        canvas.Children.Add(btn);

        mainWindow.Content = canvas;
        mainWindow.Show();
    }

    public void btn_Click(object sender, RoutedEventArgs e) {
        WinForms.OpenFileDialog ofd = new WinForms.OpenFileDialog();
        ofd.InitialDirectory = "C:\\HandsOnLab\\Embed";
        if (ofd.ShowDialog() == WinForms.DialogResult.OK) {
            RunScript(ofd.FileName);
        }
    }
}


internal static class EntryClass {
    [System.STAThread()]
    private static void Main() {
        EmbedApp app = new EmbedApp();
        app.Run();
    }
}
