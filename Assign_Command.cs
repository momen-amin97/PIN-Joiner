using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace PIN_Joiner
{
    [Transaction(TransactionMode.Manual)]
    public class Assign_Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string ComputerName = System.Environment.MachineName;
            //if (ComputerName.StartsWith("PIN"))
            //{
                UIApplication uiapp = commandData.Application;
                Application application = uiapp.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                Window_1 window_1 = new Window_1(doc, application);
                window_1.ShowDialog();
           // }
            //else
           // {
              //  TaskDialog.Show("Revit","Sorry, This add-in is not applicable with your device");
           // }


            // RevitElements revitElements = new RevitElements();
            //List<string> ee =  revitElements.GetCategories(doc);







            #region Join Method
            //Reference _ref = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Please Select1");
            //Element element1 = doc.GetElement(_ref);

            //List<Element> elements1 = new List<Element>();
            //elements1.Add(element1);

            //Join_Methods join = new Join_Methods();
            //join.Join(doc, elements1);
            #endregion
            //TaskDialog.Show("Revit", "New Plugin Is ready!!");
            return Result.Succeeded;

        }
    }
}
