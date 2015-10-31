﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace YellowstonePathology.UI.Client
{
    public partial class ClientGroupEntry : Window, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		
		private YellowstonePathology.Business.Client.Model.ClientGroup m_ClientGroup;
        private YellowstonePathology.Business.Persistence.ObjectTracker m_ObjectTracker;		
		private YellowstonePathology.Business.Client.Model.ClientCollection m_MembersClientCollection;
        private YellowstonePathology.Business.Client.Model.ClientCollection m_SearchClientCollection;

        public ClientGroupEntry(YellowstonePathology.Business.Client.Model.ClientGroup clientGroup, YellowstonePathology.Business.Persistence.ObjectTracker objectTracker)
        {                                
            this.m_ObjectTracker = objectTracker;
            this.m_ClientGroup = clientGroup;
            this.m_MembersClientCollection = YellowstonePathology.Business.Gateway.PhysicianClientGateway.GetClientCollectionByClientGroupId(this.m_ClientGroup.ClientGroupId);
            
            InitializeComponent();

            this.DataContext = this;
            Closing += ProviderEntry_Closing;
        }

        private void ProviderEntry_Closing(object sender, CancelEventArgs e)
        {
            this.Save();
        }

        public void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}     
        
        public YellowstonePathology.Business.Client.Model.ClientGroup ClientGroup
        {
            get { return this.m_ClientGroup; }
        }   

		public YellowstonePathology.Business.Client.Model.ClientCollection MembersClientCollection
        {
			get { return this.m_MembersClientCollection; }
		}

        public YellowstonePathology.Business.Client.Model.ClientCollection SearchClientCollection
        {
            get { return this.m_SearchClientCollection; }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
		{            
            Close();            
		}        

        private void Save()
        {
			this.m_ObjectTracker.SubmitChanges(this.m_ClientGroup);
        }

        private void ButtonAddToGroup_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListViewSearchClient.SelectedItem != null)
            {
                YellowstonePathology.Business.Client.Model.Client client = (YellowstonePathology.Business.Client.Model.Client)this.ListViewSearchClient.SelectedItem;
                if (this.m_MembersClientCollection.Exists(client.ClientId) == false)
                {
                    this.m_ObjectTracker.SubmitChanges(this.m_ClientGroup);
                    int clientGroupClientId = YellowstonePathology.Business.Gateway.PhysicianClientGateway.GetLargestClientGroupClientId() + 1;
                    string objectId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                    YellowstonePathology.Business.Client.Model.ClientGroupClient clientGroupClient = new Business.Client.Model.ClientGroupClient(objectId, clientGroupClientId, client.ClientId, this.m_ClientGroup.ClientGroupId);
                    YellowstonePathology.Business.Persistence.ObjectTracker objectTracker = new YellowstonePathology.Business.Persistence.ObjectTracker();
                    objectTracker.RegisterRootInsert(clientGroupClient);
                    objectTracker.SubmitChanges(clientGroupClient);

                    this.m_MembersClientCollection = YellowstonePathology.Business.Gateway.PhysicianClientGateway.GetClientCollectionByClientGroupId(this.m_ClientGroup.ClientGroupId);
                    this.NotifyPropertyChanged("MembersClientCollection");
                }
            }
        }

        private void ButtonRemoveFromGroup_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListViewMembers.SelectedItem != null)
            {
                YellowstonePathology.Business.Client.Model.Client client = (YellowstonePathology.Business.Client.Model.Client)this.ListViewMembers.SelectedItem;
                YellowstonePathology.Business.Gateway.PhysicianClientGateway.DeleteClientGroupClient(client.ClientId, this.m_ClientGroup.ClientGroupId);
                this.m_MembersClientCollection = YellowstonePathology.Business.Gateway.PhysicianClientGateway.GetClientCollectionByClientGroupId(this.m_ClientGroup.ClientGroupId);
                this.NotifyPropertyChanged("MembersClientCollection");
            }
        }        

        private void TextBoxClientNameSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.TextBoxClientNameSearchText.Text.Length > 0)
            {
                this.m_SearchClientCollection = YellowstonePathology.Business.Gateway.PhysicianClientGateway.GetClientsByClientName(this.TextBoxClientNameSearchText.Text);
                this.NotifyPropertyChanged("SearchClientCollection");
            }
        }
    }
}
