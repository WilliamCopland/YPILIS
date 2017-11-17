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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace YellowstonePathology.UI.Billing
{	
	public partial class PatientDetailPage : UserControl, INotifyPropertyChanged 
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public delegate void NextEventHandler(object sender, EventArgs e);
		public event NextEventHandler Next;

		public delegate void BackEventHandler(object sender, EventArgs e);
		public event BackEventHandler Back;

        private YellowstonePathology.Business.Patient.Model.SVHBillingData m_SVHBillingData;
        private string m_MasterAccessionNo;

        public PatientDetailPage(string masterAccessionNo, YellowstonePathology.Business.Patient.Model.SVHBillingData svhBillingData)
		{
            this.m_MasterAccessionNo = masterAccessionNo;
            this.m_SVHBillingData = svhBillingData;
			InitializeComponent();			
			DataContext = this;
		}

        public YellowstonePathology.Business.Patient.Model.SVHBillingData SVHBillingData
        {
            get { return this.m_SVHBillingData; }
        }

		public void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}		

		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{            
            if (this.Next != null) this.Next(this, new EventArgs());            
		}        

		private void ButtonBack_Click(object sender, RoutedEventArgs e)
		{
			if (this.Back != null) this.Back(this, new EventArgs());
		}

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            YellowstonePathology.Business.OrderIdParser orderIdParser = new YellowstonePathology.Business.OrderIdParser(this.m_MasterAccessionNo);
            YellowstonePathology.Business.Document.SVHBillingDocument svhBillingDocument = new Business.Document.SVHBillingDocument(this.m_SVHBillingData);
            //svhBillingDocument.SaveAsTIF(orderIdParser);

            PrintDialog dialog = new PrintDialog();
            var doc = svhBillingDocument.Document.DocumentPaginator;
            for(int i=0; i< doc.PageCount; i++)
            {
                dialog.PrintVisual(doc.GetPage(i).Visual, "Page " + i);
            }

            MessageBox.Show("Your document has been sent to the printer.");
        }
    }
}
