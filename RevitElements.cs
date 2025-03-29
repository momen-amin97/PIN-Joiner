using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PIN_Joiner
{
    public class RevitElements : INotifyPropertyChanged
    {
        public Document document { get; set; }
        public string TypesName { get; set; }

        public List<RevitElements> Types = new List<RevitElements>();



        private bool _TypesIsChecked;
        public bool TypesIsChecked
        {
            get { return _TypesIsChecked; }
            set
            {
                if (value == _TypesIsChecked)
                    return;

                _TypesIsChecked = value;

                this.OnPropertyChanged("TypesIsChecked");

            }
        }

        public RevitElements()
        {

        }

        public RevitElements(Document document, string TypeName)
        {
            this.document = document;
            this.TypesName = TypeName;
        }


        public List<RevitElements> GetTypes(Document document)
        {
            List<Element> neededElements = new List<Element>();


            List<Element> Typessss = new FilteredElementCollector(document)
               .WhereElementIsNotElementType()
               .WhereElementIsViewIndependent()
               .Select(e => e.GetTypeId())
               .Distinct()
               .Select(e => document.GetElement(e))
               .Where(e => e is ElementType && e.Category != null && e.Category.Name != "Levels" && e.Category.Name != "Grids" &&
               e.Category.Name != "Location Data" && e.Category.Name != "RVT Links")
               .ToList();

            List<string> list = new List<string>() { "Ceilings", "Columns", "Floors", "Generic Models", "Roofs", "Structural Columns", "Structural Framing", "Structural Foundations", "Supports", "Walls", "Wall Sweeps" };

            foreach (string ww in list)
            {
                neededElements.AddRange(Typessss.FindAll(x => x.Category.Name == ww));
            }

            foreach (Element e in neededElements)
            {
                Types.Add(new RevitElements(document, e.Name));
            }

            return Types;
        }

        public List<RevitElements> GetSelectedTypes(Document document ,List<string> SelectedElements)
        {
            List<Element> neededElements = new List<Element>();
            Types = new List<RevitElements>();

            List<Element> Typessss = new FilteredElementCollector(document)
               .WhereElementIsNotElementType()
               .WhereElementIsViewIndependent()
               .Select(e => e.GetTypeId())
               .Distinct()
               .Select(e => document.GetElement(e))
               .Where(e => e is ElementType && e.Category != null && e.Category.Name != "Levels" && e.Category.Name != "Grids" &&
               e.Category.Name != "Location Data" && e.Category.Name != "RVT Links")
               .ToList();

            List<string> list = SelectedElements;

            foreach (string ww in list)
            { 

                neededElements.AddRange(Typessss.FindAll(x => x.Category.Name == ww));
            }

            foreach (Element e in neededElements)
            {
                Types.Add(new RevitElements(document, e.Name));
            }

            return Types;
        }

        public List<string> GetCategories(Document document)
        {
            List<string> categories = new List<string>();
            List<string> allCategories = new List<string>();
            List<Element> Types = new FilteredElementCollector(document)
               .WhereElementIsNotElementType()
               .WhereElementIsViewIndependent()
               .Select(e => e.GetTypeId())
               .Distinct()
               .Select(e => document.GetElement(e))
               .Where(e => e is ElementType && e.Category != null && e.Category.Name != "Levels" && e.Category.Name != "Grids" &&
               e.Category.Name != "Location Data" && e.Category.Name != "RVT Links")
               .ToList();

            foreach (Element e in Types)
            {

                var ewe = e.Category.Id;
                var ee = document.GetElement(ewe);
                categories.Add(e.Category.Name);
            }
            string wee = BuiltInCategory.OST_Walls.ToString();

            foreach (String cat in categories.Distinct().OrderBy(name => name).ToList())
            {
                allCategories.Add(cat);
            }



            return allCategories;
        }


        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
