using UnityEngine;
using System.Collections;
using System.IO;
using System.Diagnostics;
using UnityEditor;

public class InkscapeWrapper : MonoBehaviour {

    //commandline 
    //inkscape test.svg -e output.png -w5000 -h5000

    //path to inkscape executable
    static private string inkscapeExe = @"C:\Program Files\Inkscape\inkscape";
    private string svgPath;
    private string outputPath;
    private bool setUp = true;

    public bool svgToPng(string svgFile, string pngFile, int width, int height)
    {
        if (!setUp) return false;

        if (width > 10000 || height > 10000) return false;

        string svgFullPath = svgPath + svgFile;
        string pngFullPath = outputPath + pngFile;

        if (!File.Exists(svgFullPath)) return false;

        string inkscapeArgs = string.Format(@"""{0}"" -e ""{1}"" -w {2} -h {3}", svgFullPath, pngFullPath, width, height);

        System.Console.WriteLine("About to run inkscape with args : " + inkscapeArgs);

        //run inkscape process
        System.Console.WriteLine("Starting SVG->PNG conversion for file : " + svgFullPath);

        ProcessStartInfo info = new ProcessStartInfo
        {
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            FileName = inkscapeExe,
            Arguments = inkscapeArgs
        };

        Process inkscape = Process.Start(info);
        inkscape.WaitForExit();
        int result = inkscape.ExitCode;

        if (result != 0)
        {
            System.Console.WriteLine("Conversion failed");
            return false;
        }
        else
        {
            System.Console.WriteLine("Conversion complete, created PNG file : " + pngFullPath);
            AssetDatabase.Refresh();
            return true;
        }

    }

    // Use this for initialization
    void Start () {

        if (!File.Exists(inkscapeExe + ".exe")) setUp = false;

        svgPath = Application.dataPath + "/svg/";

        if (!Directory.Exists(svgPath))
        {
            System.Console.WriteLine("svg path does not exist");
            Directory.CreateDirectory(svgPath);
        }

        outputPath = svgPath + "output/";

        if (!File.Exists(outputPath))
        {
            System.Console.WriteLine("png output path does not exist");
            Directory.CreateDirectory(outputPath);
        }

    }

}
