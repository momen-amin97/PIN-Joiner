using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace PIN_Joiner
{
    internal class Apps : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // CREATING TAB
            string myTab = "Pinnacle" + '\u207A';
            try
            {
                application.CreateRibbonTab(myTab);
            }
            catch
            {
            }

            // CREATING PANEL
            RibbonPanel ribPanel = application.CreateRibbonPanel(myTab, "P-Joiner");

            // CREATING BUTTON
            string assemblyName = Assembly.GetExecutingAssembly().Location;
            PushButtonData buttonData = new PushButtonData("P-Joiner", "Join", assemblyName, "PIN_Joiner.Assign_Command")
            {
                ToolTip = "P-Joiner Tool",
                LongDescription = "This Tool For Join number of Elements in same time",
            };

            // PUSHING THE BUTTON TO THE PANEL
            PushButton button = ribPanel.AddItem(buttonData) as PushButton;

            // SETTING AN ICON FOR THE BUTTON
            Uri uri = new Uri("pack://application:,,,/PIN-Joiner;component/Resources/P-Joints.png");
            button.LargeImage = new BitmapImage(uri);

            return Result.Succeeded;
        }
    }
}
