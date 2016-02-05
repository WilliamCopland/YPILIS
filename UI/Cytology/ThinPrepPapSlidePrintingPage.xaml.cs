﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace YellowstonePathology.UI.Cytology
{
	/// <summary>
	/// Interaction logic for ThinPrepPapSlidePrintingPage.xaml
	/// </summary>
	public partial class ThinPrepPapSlidePrintingPage : UserControl, INotifyPropertyChanged, Business.Interface.IPersistPageChanges
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public delegate void FinishedEventHandler(object sender, EventArgs e);
		public event FinishedEventHandler Finished;

        public delegate void PageTimedOutEventHandler(object sender, EventArgs e);
        public event PageTimedOutEventHandler PageTimedOut;

        private System.Windows.Threading.DispatcherTimer m_PageTimeOutTimer;

        private YellowstonePathology.Business.Specimen.Model.SpecimenOrder m_SpecimenOrder;
        private YellowstonePathology.Business.Test.AccessionOrder m_AccessionOrder;        
        private YellowstonePathology.Business.User.SystemIdentity m_SystemIdentity;

        private YellowstonePathology.Business.BarcodeScanning.BarcodeScanPort m_BarcodeScanPort;
		private string m_PageHeaderText;        

		public ThinPrepPapSlidePrintingPage(YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder, 
            YellowstonePathology.Business.Test.AccessionOrder accessionOrder,        
            YellowstonePathology.Business.User.SystemIdentity systemIdentity)
		{
            this.m_SpecimenOrder = specimenOrder;
            this.m_AccessionOrder = accessionOrder;            
            this.m_SystemIdentity = systemIdentity;
			this.m_PageHeaderText = "Slide Printing Page ";

            this.m_BarcodeScanPort = Business.BarcodeScanning.BarcodeScanPort.Instance;

            this.m_PageTimeOutTimer = new System.Windows.Threading.DispatcherTimer();
            this.m_PageTimeOutTimer.Interval = new TimeSpan(0, 20, 0);
            this.m_PageTimeOutTimer.Tick += new EventHandler(PageTimeOutTimer_Tick);

            InitializeComponent();

			DataContext = this;            

            this.Loaded += new RoutedEventHandler(ThinPrepPapSlidePrintingPage_Loaded);
            this.Unloaded += new RoutedEventHandler(ThinPrepPapSlidePrintingPage_Unloaded);
		}

        private void ThinPrepPapSlidePrintingPage_Loaded(object sender, RoutedEventArgs e)
        {            
            this.m_BarcodeScanPort.ThinPrepSlideScanReceived += new Business.BarcodeScanning.BarcodeScanPort.ThinPrepSlideScanReceivedHandler(BarcodeScanPort_ThinPrepSlideScanReceived);
            this.m_BarcodeScanPort.AliquotOrderIdReceived += new Business.BarcodeScanning.BarcodeScanPort.AliquotOrderIdReceivedHandler(BarcodeScanPort_AliquotOrderIdReceived);
            this.m_PageTimeOutTimer.Start();
        }

        private void ThinPrepPapSlidePrintingPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.m_BarcodeScanPort.ThinPrepSlideScanReceived -= BarcodeScanPort_ThinPrepSlideScanReceived;
            this.m_BarcodeScanPort.AliquotOrderIdReceived -= BarcodeScanPort_AliquotOrderIdReceived;
            this.m_PageTimeOutTimer.Stop();
        }

        private void PageTimeOutTimer_Tick(object sender, EventArgs e)
        {
            this.m_PageTimeOutTimer.Stop();

            EventArgs eventArgs = new EventArgs();
            this.PageTimedOut(this, eventArgs);
        }

        private void BarcodeScanPort_AliquotOrderIdReceived(string scanData)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {                        
                        if (this.m_SpecimenOrder.AliquotOrderCollection.Exists(scanData) == true)
                        {
                            YellowstonePathology.Business.Facility.Model.FacilityCollection facilityCollection = Business.Facility.Model.FacilityCollection.GetAllFacilities();
                            YellowstonePathology.Business.Facility.Model.LocationCollection locationCollection = YellowstonePathology.Business.Facility.Model.LocationCollection.GetAllLocations();
                            YellowstonePathology.Business.Facility.Model.Facility thisFacility = facilityCollection.GetByFacilityId(YellowstonePathology.Business.User.UserPreferenceInstance.Instance.UserPreference.FacilityId);
                            YellowstonePathology.Business.Facility.Model.Location thisLocation = locationCollection.GetLocation(YellowstonePathology.Business.User.UserPreferenceInstance.Instance.UserPreference.LocationId);

                            YellowstonePathology.Business.Test.AliquotOrder aliquotOrder = this.m_SpecimenOrder.AliquotOrderCollection.GetByAliquotOrderId(scanData);
                            aliquotOrder.Validate(this.m_SystemIdentity);

                            string objectId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                            YellowstonePathology.Business.MaterialTracking.Model.MaterialTrackingLog materialTrackingLog = new Business.MaterialTracking.Model.MaterialTrackingLog(objectId, scanData, null, thisFacility.FacilityId, thisFacility.FacilityName,
                                thisLocation.LocationId, thisLocation.Description, this.m_SystemIdentity.User.UserId, this.m_SystemIdentity.User.UserName, "Panther Aliquot Scanned", "Panther aliquot scanned at cytology aliquoting", "Aliquot", this.m_AccessionOrder.MasterAccessionNo, aliquotOrder.Label, aliquotOrder.ClientAccessioned);
                            YellowstonePathology.Business.Persistence.ObjectGateway.Instance.SubmitRootInsert(materialTrackingLog);                            

                            if (this.m_SpecimenOrder.AliquotOrderCollection.HasUnvalidatedItems() == false)
                            {
                                this.Save(false);
                                this.Finished(this, new EventArgs());
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("The aliquot scanned does not appear to belong to this specimen.");
                        }                                             
                    }));   
        }

        private void BarcodeScanPort_ThinPrepSlideScanReceived(Business.BarcodeScanning.Barcode barcode)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        if (barcode.IsValidated == true)
                        {
                            if (this.m_SpecimenOrder.AliquotOrderCollection.Exists(barcode.ID) == true)
                            {
                                YellowstonePathology.Business.Facility.Model.FacilityCollection facilityCollection = Business.Facility.Model.FacilityCollection.GetAllFacilities();
                                YellowstonePathology.Business.Facility.Model.LocationCollection locationCollection = YellowstonePathology.Business.Facility.Model.LocationCollection.GetAllLocations();
                                YellowstonePathology.Business.Facility.Model.Facility thisFacility = facilityCollection.GetByFacilityId(YellowstonePathology.Business.User.UserPreferenceInstance.Instance.UserPreference.FacilityId);
                                YellowstonePathology.Business.Facility.Model.Location thisLocation = locationCollection.GetLocation(YellowstonePathology.Business.User.UserPreferenceInstance.Instance.UserPreference.LocationId);                                

                                YellowstonePathology.Business.Test.AliquotOrder aliquotOrder = this.m_SpecimenOrder.AliquotOrderCollection.GetByAliquotOrderId(barcode.ID);
                                aliquotOrder.Validate(this.m_SystemIdentity);

                                string objectId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                                YellowstonePathology.Business.MaterialTracking.Model.MaterialTrackingLog materialTrackingLog = new Business.MaterialTracking.Model.MaterialTrackingLog(objectId, barcode.ID, null, thisFacility.FacilityId, thisFacility.FacilityName,
                                    thisLocation.LocationId, thisLocation.Description, this.m_SystemIdentity.User.UserId, this.m_SystemIdentity.User.UserName, "Slide Scanned", "Slide scanned at cytology aliquoting", "Aliquot", this.m_AccessionOrder.MasterAccessionNo, aliquotOrder.Label, aliquotOrder.ClientAccessioned);
                                YellowstonePathology.Business.Persistence.ObjectGateway.Instance.SubmitRootInsert(materialTrackingLog);

                                if (this.m_SpecimenOrder.AliquotOrderCollection.HasUnvalidatedItems() == false)
                                {
                                    this.Save(false);
                                    this.Finished(this, new EventArgs());
                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("The aliquot scanned does not appear to belong to this specimen.");
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("The scanner did not read the label correctly.", "Scan not successful.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }));    
        }

		public string PageHeaderText
		{
			get { return this.m_PageHeaderText; }
		}

        public YellowstonePathology.Business.Specimen.Model.SpecimenOrder SpecimenOrder
        {
            get { return this.m_SpecimenOrder; }
        }

        public YellowstonePathology.Business.Test.AccessionOrder AccessionOrder
        {
            get { return this.m_AccessionOrder; }
        }

		public bool OkToSaveOnNavigation(Type pageNavigatingTo)
		{
			return false;
		}

		public bool OkToSaveOnClose()
		{
			return true;
		}

		public void Save(bool releaseLock)
		{
            YellowstonePathology.Business.Persistence.ObjectGateway.Instance.SubmitChanges(this.m_AccessionOrder, releaseLock);
        }

		public void UpdateBindingSources()
		{

		}

		public void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}		

        private void ButtonAddThinPrepSlide_Click(object sender, RoutedEventArgs e)
        {            
            YellowstonePathology.Business.Specimen.Model.ThinPrepSlide thinPrepSlide = new Business.Specimen.Model.ThinPrepSlide();
            if (this.m_SpecimenOrder.AliquotOrderCollection.Exists(thinPrepSlide) == false)
            {
                YellowstonePathology.Business.Test.AliquotOrder aliquotOrder = this.m_SpecimenOrder.AliquotOrderCollection.AddAliquot(thinPrepSlide, this.m_SpecimenOrder, this.m_AccessionOrder.AccessionDate.Value);
                this.Save(false);
            }
            else
            {
                MessageBox.Show("Cannot add another Thin Prep Slide as one already exists.");
            }            
        }

        private void PrintThinPrepSlide(YellowstonePathology.Business.Test.AliquotOrder aliquotOrder)
        {
            YellowstonePathology.Business.BarcodeScanning.BarcodeVersion2 barcode = new YellowstonePathology.Business.BarcodeScanning.BarcodeVersion2(Business.BarcodeScanning.BarcodePrefixEnum.PSLD, aliquotOrder.AliquotOrderId);
            YellowstonePathology.Business.BarcodeScanning.CytycBarcode cytycBarcode = YellowstonePathology.Business.BarcodeScanning.CytycBarcode.Parse(this.m_AccessionOrder.MasterAccessionNo);
            YellowstonePathology.Business.Label.Model.ThinPrepSlide thinPrepSlide = new Business.Label.Model.ThinPrepSlide(this.m_AccessionOrder.PFirstName, this.m_AccessionOrder.PLastName, barcode, cytycBarcode);
            YellowstonePathology.Business.Label.Model.ThinPrepSlidePrinter thinPrepSlidePrinter = new Business.Label.Model.ThinPrepSlidePrinter();
            thinPrepSlidePrinter.Queue.Enqueue(thinPrepSlide);
            thinPrepSlidePrinter.Print();
        }

        private void ButtonAddPantherAliquot_Click(object sender, RoutedEventArgs e)
        {
            YellowstonePathology.Business.Specimen.Model.PantherAliquot pantherAliquot = new Business.Specimen.Model.PantherAliquot();
            if (this.m_SpecimenOrder.AliquotOrderCollection.Exists(pantherAliquot) == false)
            {
                YellowstonePathology.Business.Test.AliquotOrder aliquotOrder = this.m_SpecimenOrder.AliquotOrderCollection.AddAliquot(pantherAliquot, this.m_SpecimenOrder, this.m_AccessionOrder.AccessionDate.Value);                
            }
            else
            {
                MessageBox.Show("Cannot add another Panther Aliquot as one already exists.");
            }
        }

        private void ButtonReprintSelected_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListBoxAliquots.SelectedItem != null)
            {
                YellowstonePathology.Business.Test.AliquotOrder aliquotOrder = (YellowstonePathology.Business.Test.AliquotOrder)this.ListBoxAliquots.SelectedItem;
                YellowstonePathology.Business.Specimen.Model.ThinPrepSlide thinPrepSlide = new Business.Specimen.Model.ThinPrepSlide();
                YellowstonePathology.Business.Specimen.Model.PantherAliquot pantherAliquot = new Business.Specimen.Model.PantherAliquot();

                if (aliquotOrder.AliquotType == thinPrepSlide.AliquotType)
                {
                    this.PrintThinPrepSlide(aliquotOrder);
                }
                else if (aliquotOrder.AliquotType == pantherAliquot.AliquotType)
                {
                    YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder = this.m_AccessionOrder.SpecimenOrderCollection.GetSpecimenOrderByAliquotOrderId(aliquotOrder.AliquotOrderId);
                    YellowstonePathology.Business.Label.Model.PantherLabel pantherLabel = new Business.Label.Model.PantherLabel(aliquotOrder.AliquotOrderId, this.m_AccessionOrder.PatientDisplayName, this.m_AccessionOrder.PBirthdate.Value, specimenOrder.Description);
                    YellowstonePathology.Business.Label.Model.MolecularLabelPrinter molecularLabelPrinter = new Business.Label.Model.MolecularLabelPrinter();
                    molecularLabelPrinter.Queue.Enqueue(pantherLabel);
                    molecularLabelPrinter.Print();
                }
            }
        }

        private void ButtonFinished_Click(object sender, RoutedEventArgs e)
        {
            this.Save(false);
            if (this.Finished != null) this.Finished(this, new EventArgs());
        }        

        private void ListBoxAliquots_MouseUp(object sender, MouseButtonEventArgs e)
        {         
            if (this.ListBoxAliquots.SelectedItems.Count != 0)
            {
                YellowstonePathology.Business.Specimen.Model.ThinPrepSlide thinPrepSlide = new Business.Specimen.Model.ThinPrepSlide();
                YellowstonePathology.Business.Specimen.Model.PantherAliquot pantherAliquot = new Business.Specimen.Model.PantherAliquot();

                YellowstonePathology.Business.Test.AliquotOrder aliquotOrder = (YellowstonePathology.Business.Test.AliquotOrder)this.ListBoxAliquots.SelectedItem;
                if (aliquotOrder.Status == YellowstonePathology.Business.Slide.Model.SlideStatusEnum.Created.ToString())
                {
                    if (aliquotOrder.AliquotType == thinPrepSlide.AliquotType)
                    {                    
                        this.PrintThinPrepSlide(aliquotOrder);                        
                    }
                    else if(aliquotOrder.AliquotType == pantherAliquot.AliquotType)
                    {
                        YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder = this.m_AccessionOrder.SpecimenOrderCollection.GetSpecimenOrderByAliquotOrderId(aliquotOrder.AliquotOrderId);
                        YellowstonePathology.Business.Label.Model.PantherLabel pantherLabel = new Business.Label.Model.PantherLabel(aliquotOrder.AliquotOrderId, this.m_AccessionOrder.PatientDisplayName, this.m_AccessionOrder.PBirthdate.Value, specimenOrder.Description);
                        YellowstonePathology.Business.Label.Model.MolecularLabelPrinter molecularLabelPrinter = new Business.Label.Model.MolecularLabelPrinter();
                        molecularLabelPrinter.Queue.Enqueue(pantherLabel);
                        molecularLabelPrinter.Print();
                    }
                    aliquotOrder.Status = YellowstonePathology.Business.TrackedItemStatusEnum.Printed.ToString();             
                }

                this.NotifyPropertyChanged(string.Empty);
            }
        }
	}
}
