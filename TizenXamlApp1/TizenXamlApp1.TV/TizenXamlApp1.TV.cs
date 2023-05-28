using System;
using Xamarin.Forms;

[assembly: ExportFont("AppleSDGothicNeoM.ttf", Alias = "AppleFont")]
[assembly: ExportFont("AppleSDGothicNeoB.ttf", Alias = "AppleFont_B")]
[assembly: ExportFont("EliceDigitalBaeum_Regular.ttf", Alias = "EliceFont")]
[assembly: ExportFont("Struggle_faith_Regular.ttf", Alias = "FaithFont")]

namespace TizenXamlApp1
{
    class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            LoadApplication(new App());
        }
        
        static void Main(string[] args)
        {
            var app = new Program();
            Forms.Init(app);
            app.Run(args);
        }
    }
}
