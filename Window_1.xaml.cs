using Autodesk.Revit.DB;
using System.Collections.Generic;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PIN_Joiner
{
    /// <summary>
    /// Interaction logic for Window_1.xaml
    /// </summary>
    public partial class Window_1 : Window
    {
        Document document { get; set; }
        List<Element> ProjectLevels = new List<Element>();

        public LevelsClass levels = new LevelsClass();

        public Application application { get; set; }

        public RevitElements elements = new RevitElements();


        System.Collections.IList SelectedItems = null;
        System.Collections.IList SecondItems = null;

        public Window_1(Document document , Application application)
        {
            InitializeComponent();
            this.application = application;
            SelectedItems = FirstElements.SelectedItems;
            SecondItems = SecondElements.SelectedItems;

            ProjectLevels = new FilteredElementCollector(document)
                                             .OfCategory(BuiltInCategory.OST_Levels)
                                             .WhereElementIsNotElementType()
                                             .ToElements().ToList();

            levels.levelsNames = levels.GetLevelsNames(document, ProjectLevels);
            foreach (LevelsClass level in levels.levelsNames)
            {
                level.IsChecked = true;
            }

            elements.Types = elements.GetTypes(document);
            foreach (RevitElements type in elements.Types)
            {
                type.TypesIsChecked = true;
            }

            //List<string> list = revitElements.GetCategories(document);
            List<string> list = new List<string>() { "Ceilings", "Columns", "Floors", "Generic Models", "Roofs", "Structural Columns", "Structural Framings", "Structural Foundations", "Walls", "Wall Sweeps" };

            List<string> list2 = new List<string>() { "Ceilings", "Columns", "Floors", "Generic Models", "Roofs", "Structural Columns", "Structural Framings", "Structural Foundations", "Walls" };
            this.FirstElements.ItemsSource = list;
            this.SecondElements.ItemsSource = list2;
            this.document = document;


        }


        enum Categories
        {
            Walls = BuiltInCategory.OST_Walls,
            Ceilings = BuiltInCategory.OST_Ceilings,
            Floors = BuiltInCategory.OST_Floors,
            StructuralColumns = BuiltInCategory.OST_StructuralColumns,
            ArchitecturalColumns = BuiltInCategory.OST_Columns,
            Roofs = BuiltInCategory.OST_Roofs,
        }

        private void Method(int index)
        {
            List<string> SelectedLevels = new List<string>();
            SelectedLevels = Selectedlevelss();


            List<string> SelectedTypes = new List<string>();
            SelectedTypes = Selectedelementss();

            var DistinctSelectedTypes = SelectedTypes.Distinct();
            List<Element> elements = new List<Element>();
            List<Element> elementsWithSelectedTypes = new List<Element>();
            List<Element> elementsWithSelectedLevels = new List<Element>();

            foreach (string item in SelectedItems)
            {
                switch (item)
                {
                    case "Walls":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Walls)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Ceilings":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Ceilings)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Floors":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Floors)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Columns":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Columns)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Structural Columns":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Roofs":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Roofs)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Structural Framings":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Generic Models":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_GenericModel)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Structural Foundations":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                    case "Wall Sweeps":
                        elements.AddRange(new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Cornices)).WhereElementIsNotElementType().ToElements().ToList());
                        break;
                }
            }
            foreach (string elem in DistinctSelectedTypes)
            {
                elementsWithSelectedTypes.AddRange(elements.FindAll(x => x.Name == elem));
            }
            if (SelectedLevels.Count == ProjectLevels.Count)
            {
                elementsWithSelectedLevels = elementsWithSelectedTypes;
            }
            else
            {
                foreach (string elem in SelectedLevels)
                {
                    var Level = ProjectLevels.Find(x => x.Name == elem);
                    elementsWithSelectedLevels.AddRange(elementsWithSelectedTypes.FindAll(x => x.LevelId == Level.Id));

                    if (SelectedItems.Contains("Structural Framings"))
                    {
                        List<Element> beams = new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming))
                        .WhereElementIsNotElementType().ToElements().Where(ee => elem.Contains(ee.LookupParameter("Reference Level").AsValueString()) == true).ToList();

                        if (beams.Count != 0)
                            elementsWithSelectedLevels.AddRange(beams);
                    }
                    if (SelectedItems.Contains("Generic Models"))
                    {
                        List<Element> Genericmodel = new FilteredElementCollector(document).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_GenericModel))
                        .WhereElementIsNotElementType().ToElements().Where(ee => ee.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsValueString() == elem).ToList();

                        if (Genericmodel.Count != 0)
                            elementsWithSelectedLevels.AddRange(Genericmodel);
                    }
                }
            }

            if (index == 1)
            {
                Join_Methods join_methods = new Join_Methods(document, application);
                join_methods.Join(document, elementsWithSelectedLevels, SecondItems);
            }
            else if (index == 2)
            {
                Join_Methods join_methods = new Join_Methods(document, application);
                join_methods.UnJoin(document, elementsWithSelectedLevels, SecondItems);
            }
            else if (index == 3)
            {
                Join_Methods join_methods = new Join_Methods(document, application);
                join_methods.SwitchJoin(document, elementsWithSelectedLevels, SecondItems);
            }

        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            Method(1);
        }


        private void UnJoin_Click(object sender, RoutedEventArgs e)
        {
            Method(2);
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            Method(3);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.Levels.ItemsSource = levels.levelsNames;
            this.Types.ItemsSource = elements.Types;
        }

        private List<string> Selectedelementss()
        {
            List<string> SelectedTypes = new List<string>();
            foreach (RevitElements elems in elements.Types)
            {
                if (elems.TypesIsChecked == true)
                {

                    SelectedTypes.Add(elems.TypesName);
                }
            }
            return SelectedTypes;
        }

        private List<string> Selectedlevelss()
        {
            List<string> SelectedLevels = new List<string>();
            foreach (LevelsClass levels in levels.levelsNames)
            {
                if (levels.IsChecked == true)
                {

                    SelectedLevels.Add(levels.Name);
                }
            }
            return SelectedLevels;
        }

        private void Advanced(object sender, RoutedEventArgs e)
        {
            if (Expander.IsExpanded == false)
            {
                this.Expander.IsExpanded = true;
            }
            else
            {
                Expander.IsExpanded = false;
            }
        }

        private void Selection_Changed_Event(object sender, SelectionChangedEventArgs e)
        {
            if (Expander.IsExpanded == false)
            {
                this.Expander.IsExpanded = true;
            }
            List<string> SelectedCategories = new List<string>();
            foreach (var ei in SelectedItems)
            {
                SelectedCategories.Add(ei.ToString());
            }

            var wwww = elements.GetSelectedTypes(document, SelectedCategories);
            foreach (var www in wwww)
            {
                www.TypesIsChecked = true;
            }
            this.Types.ItemsSource = wwww;

        }
    }
}
