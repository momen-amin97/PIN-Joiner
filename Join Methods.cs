using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using eTransmitForRevitDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace PIN_Joiner
{
    internal class Join_Methods
    {
        double Counter { get; set; }

        private Application application { get; set; }

        private string m_currentDocName;

        public string CurrentDocName
        {
            get
            {
                return m_currentDocName;
            }
            set
            {
                m_currentDocName = value;
            }
        }

        //private EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs> m_handler;

        //private Dictionary<string, HashSet<FailureStorage>> m_errors;
        //public Dictionary<string, HashSet<FailureStorage>> Errors => m_errors;
        public RevitProgressBar revitProgressBar { get; set; }
        public ProgressBar revitProgressViewer { get; set; }

        enum Categories
        {
            Walls = BuiltInCategory.OST_Walls,
            Ceilings = BuiltInCategory.OST_Ceilings,
            Floors = BuiltInCategory.OST_Floors,
            StructuralColumns = BuiltInCategory.OST_StructuralColumns,
            ArchitecturalColumns = BuiltInCategory.OST_Columns,
            Roofs = BuiltInCategory.OST_Roofs,
        }

        public Join_Methods(Document document , Application application)
        {
            //this.Docs = doc;
            this.application = application;
            this.CurrentDocName = document.PathName;

            //m_handler = revitDoc_failureProcessing;
            //application.FailuresProcessing += m_handler;
            //m_errors = new Dictionary<string, HashSet<FailureStorage>>();
        }

        public void Join(Document doc, List<Element> SelectedElements, System.Collections.IList SecondItems)
        {
            int counter = 0;
            int joined = 0;
            int notjoined = 0;
            revitProgressBar = new RevitProgressBar();
            revitProgressViewer = new ProgressBar(doc);
            Counter = 1;

            foreach (Element element in SelectedElements)
            {
                GeometryElement geomElement = element.get_Geometry(new Options());
                Solid solid = null;
                foreach (GeometryObject geomObj in geomElement)
                {
                    solid = geomObj as Solid;
                    if (solid != null) break;

                }
                if (solid != null)
                {
                    List<Element> IntersectedElements2 = new FilteredElementCollector(doc).WherePasses(new ElementIntersectsSolidFilter(solid)).ToList();

                    foreach (Element Intersectedelement in IntersectedElements2)
                    {
                        using (Transaction transaction = new Transaction(doc))
                        {
                            transaction.Start("Join");
                            foreach (string categNames in SecondItems)
                            {
                                if (Intersectedelement.Category.Name == categNames)
                                {
                                    try
                                    {
                                        JoinGeometryUtils.JoinGeometry(doc, element, Intersectedelement);
                                        joined++;
                                    }
                                    catch
                                    { notjoined++; }
                                    ProgressBar(counter,joined, notjoined, SelectedElements.Count, 1);
                                }
                            }
                            transaction.Commit();
                        }
                    }
                }
                counter++;
            }
            FinalizeProgressBar();
        }

        public void UnJoin(Document doc, List<Element> SelectedElements, System.Collections.IList SecondItems)
        {
            int counter = 0;
            int joined = 0;
            int notjoined = 0;
            revitProgressBar = new RevitProgressBar();
            revitProgressViewer = new ProgressBar(doc);
            Counter = 1;

            foreach (Element element in SelectedElements)
            {
                List<ElementId> IntersectedElements2 = JoinGeometryUtils.GetJoinedElements(doc, element).ToList();

                foreach (ElementId Intersectedelement in IntersectedElements2)
                {
                    using (Transaction transaction = new Transaction(doc))
                    {
                        transaction.Start("UnJoin");
                        foreach (string categNames in SecondItems)
                        {

                            Element elem = doc.GetElement(Intersectedelement);
                            if (elem.Category.Name == categNames)
                            {
                                try
                                {
                                    JoinGeometryUtils.UnjoinGeometry(doc, element, elem);
                                    joined++;
                                }
                                catch
                                { notjoined++; }
                                ProgressBar(counter,joined, notjoined, SelectedElements.Count, 2);
                            }
                        }

                        transaction.Commit();
                    }
                }
                counter++;
            }
            FinalizeProgressBar();
        }


        public void SwitchJoin(Document doc, List<Element> SelectedElements, System.Collections.IList SecondItems)
        {
            int counter = 0;
            int joined = 0;
            int notjoined = 0;
            revitProgressBar = new RevitProgressBar();
            revitProgressViewer = new ProgressBar(doc);
            Counter = 1;

            foreach (Element element in SelectedElements)
            {
                List<ElementId> IntersectedElements2 = JoinGeometryUtils.GetJoinedElements(doc, element).ToList();

                foreach (ElementId Intersectedelement in IntersectedElements2)
                {
                    using (Transaction transaction = new Transaction(doc))
                    {
                        transaction.Start("Switch join");
                        foreach (string categNames in SecondItems)
                        {

                            Element elem = doc.GetElement(Intersectedelement);
                            if (elem.Category.Name == categNames)
                            {
                                try
                                {
                                    JoinGeometryUtils.SwitchJoinOrder(doc, element, elem);
                                    joined++;
                                }
                                catch
                                { notjoined++; }
                                ProgressBar(counter ,joined, notjoined, SelectedElements.Count, 3);
                            }
                        }

                        transaction.Commit();
                    }
                }
                counter++;
            }
            FinalizeProgressBar();
        }

        public void FinalizeProgressBar()
        {
            Counter = 100;
            revitProgressViewer.revitProgressViewer.Progress = Counter;
            revitProgressViewer.pBar.Value = Counter;
            revitProgressViewer.Show();
        }

        public void ProgressBar(int counter ,int Joined, int NotJoined, double NoOfElements, int type)
        {

            #region ProgressScreenCounters
            double totalProcesses = Joined + NotJoined;
            double Percentage = (counter / NoOfElements) * 100;
            Counter = Percentage;
            //fileCount++;
            #endregion
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke((() =>
            {
                if (type == 1)
                {

                    revitProgressViewer.revitProgressViewer.CurrentElementNumber = $"Joined : {Joined}/ Not Joined : {NotJoined}";
                    //revitProgressViewer.Title = $"{CurrentFileNumber}/{RevitFilesNumber} - Current File: {CurrentFileName}";
                    revitProgressViewer.revitProgressViewer.Progress = Counter;
                    revitProgressViewer.pBar.Value = Counter;
                    revitProgressViewer.Show();
                }
                else if (type == 2)
                {
                    revitProgressViewer.revitProgressViewer.CurrentElementNumber = $"UnJoined : {Joined}/ Still Joined : {NotJoined}";
                    //revitProgressViewer.Title = $"{CurrentFileNumber}/{RevitFilesNumber} - Current File: {CurrentFileName}";
                    revitProgressViewer.revitProgressViewer.Progress = Counter;
                    revitProgressViewer.pBar.Value = Counter;
                    revitProgressViewer.Show();
                }
                else if (type == 3)
                {
                    revitProgressViewer.revitProgressViewer.CurrentElementNumber = $"Join Switched  : {Joined}/ Not Switched : {NotJoined}";
                    //revitProgressViewer.Title = $"{CurrentFileNumber}/{RevitFilesNumber} - Current File: {CurrentFileName}";
                    revitProgressViewer.revitProgressViewer.Progress = Counter;
                    revitProgressViewer.pBar.Value = Counter;
                    revitProgressViewer.Show();
                }
            }), DispatcherPriority.ApplicationIdle);



        }

        public void revitDoc_failureProcessing(object sender, Autodesk.Revit.DB.Events.FailuresProcessingEventArgs e)
        {
            //    try
            //    {
            //        bool flag = true;
            //        bool flag2 = false;
            //        Autodesk.Revit.DB.FailuresAccessor failuresAccessor = e.GetFailuresAccessor();
            //        string currentDocName = CurrentDocName;
            //        HashSet<FailureStorage> hashSet = m_errors[currentDocName];
            //        IList<Autodesk.Revit.DB.FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
            //        foreach (Autodesk.Revit.DB.FailureMessageAccessor item in failureMessages)
            //        {
            //            hashSet.Add(new FailureStorage(item, failuresAccessor.GetDocument()));
            //            if (item.GetSeverity() == Autodesk.Revit.DB.FailureSeverity.DocumentCorruption)
            //            {
            //                flag = false;
            //                failuresAccessor.RollBackPendingTransaction();
            //            }
            //            else if (item.GetSeverity() == Autodesk.Revit.DB.FailureSeverity.Warning)
            //            {
            //                failuresAccessor.DeleteWarning(item);
            //            }
            //            else if (!item.HasResolutions())
            //            {
            //                flag = false;
            //                failuresAccessor.RollBackPendingTransaction();
            //            }
            //            else
            //            {
            //                flag2 = true;
            //                failuresAccessor.ResolveFailure(item);
            //            }
            //        }

            //        if (flag && flag2)
            //        {
            //            e.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        application.WriteJournalComment("Pinnacle - can't handle a warning or error. Extended message follows: " + ex.Message, timeStamp: true);
            //    }
        }
    }
}



