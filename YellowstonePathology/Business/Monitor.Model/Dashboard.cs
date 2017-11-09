﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowstonePathology.Business.Persistence;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.WebServices.Data;

namespace YellowstonePathology.Business.Monitor.Model
{
    [PersistentClass("tblDashboard", "YPIDATA")]
    public class Dashboard : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;        

        private DateTime m_DashboardDate;
        private string m_YPIBlockCount;
        private int m_YPIBlocks;
        private string m_BozemanBlockCount;
        private int m_BozemanBlocks;
        private MonitorStateEnum m_State;

        public Dashboard()
        {
            this.m_State = MonitorStateEnum.Normal;
        }

        [PersistentPrimaryKeyProperty(false)]
        public DateTime DashboardDate
        {
            get { return this.m_DashboardDate; }
            set
            {
                if (this.m_DashboardDate != value)
                {
                    this.m_DashboardDate = value;
                    this.NotifyPropertyChanged("DashboardDate");
                }
            }
        }

        [PersistentProperty()]
        public string BozemanBlockCount
        {
            get { return this.m_BozemanBlockCount; }
            set
            {
                if (this.m_BozemanBlockCount != value)
                {
                    if (value == null || value == "Unknown")
                    {
                        this.m_BozemanBlockCount = "Unknown";
                        this.m_BozemanBlocks = 0;
                    }
                    else
                    {
                        this.m_BozemanBlockCount = value;
                        this.m_BozemanBlocks = Convert.ToInt32(value);
                    }
                    this.NotifyPropertyChanged("BozemanBlockCount");
                    this.NotifyPropertyChanged("TotalBlockCount");
                }
            }
        }

        public string YPIBlockCount
        {
            get { return this.m_YPIBlockCount; }
            set
            {
                if (this.m_YPIBlockCount != value)
                {
                    this.m_YPIBlockCount = value;
                    this.NotifyPropertyChanged("YPIBlockCount");
                    this.NotifyPropertyChanged("TotalBlockCount");
                }
            }
        }

        public int YPIBlocks
        {
            get { return this.m_YPIBlocks; }
            set
            {
                this.m_YPIBlocks = value;
                this.YPIBlockCount = this.m_YPIBlocks.ToString();
            }
        }

        public int TotalBlockCount
        {
            get { return this.m_YPIBlocks + this.m_BozemanBlocks; }
        }             

        public void SetBozemanBlockCount()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            service.Credentials = new WebCredentials("ypiilab\\blockcount", "blockorama");

            service.TraceEnabled = true;
            service.TraceFlags = TraceFlags.All;

            service.AutodiscoverUrl("blockcount@ypii.com", RedirectionUrlValidationCallback);
            SearchFilter searchFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false);

            List<SearchFilter> searchFilterCollection = new List<SearchFilter>();
            searchFilterCollection.Add(searchFilter);
            ItemView view = new ItemView(50);
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived);
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);
            view.Traversal = ItemTraversal.Shallow;
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, searchFilter, view);

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\d{1,3}(?=\\D*$)");
            foreach (Item mailItem in findResults.Items)
            {
                if (mailItem is EmailMessage)
                {
                    System.Text.RegularExpressions.Match match = regex.Match(mailItem.Subject);
                    if (match.Captures.Count != 0)
                    {
                        this.m_BozemanBlockCount = match.Value;
                        this.m_BozemanBlocks = Convert.ToInt32(match.Value);
                        this.NotifyPropertyChanged(string.Empty);
                        mailItem.Delete(DeleteMode.MoveToDeletedItems);                        
                    }                    
                }                
            }            
        }

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }

        private static bool CertificateValidationCallBack(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
    }
}
