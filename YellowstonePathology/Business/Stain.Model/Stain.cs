﻿using System;
using System.ComponentModel;

namespace YellowstonePathology.Business.Stain.Model
{
    public class Stain : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_StainId;
        private string m_StainType;
        private string m_OrderComment;
        private string m_TestId;
        private string m_StainName;
        private string m_StainAbbreviation;
        private string m_AliquotType;
        private string m_DefaultResult;
        private string m_HistologyDisplayString;
        private string m_StainerType;
        private string m_VentanaBenchMarkProtocolName;
        private string m_CPTCode;
        private string m_SubsequentCPTCode;
        private string m_GCode;
        private int m_VentanaBenchMarkId;
        private int? m_VentanaBenchMarkWetId;
        private int m_StainResultGroupId;
        private bool m_IsBillable;
        private bool m_HasGCode;
        private bool m_IsDualOrder;
        private bool m_HasCptCodeLevels;
        private bool m_Active;
        private bool m_NeedsAcknowledgement;
        private bool m_UseWetProtocol;
        private bool m_PerformedByHand;
        private bool m_RequestForAdditionalReport;
        private bool m_HasWetProtocol;

        protected Stain m_FirstStain;
        protected Stain m_SecondStain;
        protected string m_DepricatedFirstTestId;
        protected string m_DepricatedSecondTestId;

        public Stain() { }

        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public string StainId
        {
            get { return this.m_StainId; }
            set
            {
                if (this.m_StainId != value)
                {
                    this.m_StainId = value;
                    this.NotifyPropertyChanged("StainId");
                }
            }
        }

        public string StainType
        {
            get { return this.m_StainType; }
            set
            {
                if (this.m_StainType != value)
                {
                    this.m_StainType = value;
                    this.NotifyPropertyChanged("StainType");
                }
            }
        }

        public string OrderComment
        {
            get { return this.m_OrderComment; }
            set
            {
                if (this.m_OrderComment != value)
                {
                    this.m_OrderComment = value;
                    this.NotifyPropertyChanged("OrderComment");
                }
            }
        }

        public string TestId
        {
            get { return this.m_TestId; }
            set
            {
                if (this.m_TestId != value)
                {
                    this.m_TestId = value;
                    this.NotifyPropertyChanged("TestId");
                }
            }
        }

        public string StainName
        {
            get { return this.m_StainName; }
            set
            {
                if (this.m_StainName != value)
                {
                    this.m_StainName = value;
                    this.NotifyPropertyChanged("StainName");
                }
            }
        }

        public string StainAbbreviation
        {
            get { return this.m_StainAbbreviation; }
            set
            {
                if (this.m_StainAbbreviation != value)
                {
                    this.m_StainAbbreviation = value;
                    this.NotifyPropertyChanged("StainAbbreviation");
                }
            }
        }

        public string AliquotType
        {
            get { return this.m_AliquotType; }
            set
            {
                if (this.m_AliquotType != value)
                {
                    this.m_AliquotType = value;
                    this.NotifyPropertyChanged("AliquotType");
                }
            }
        }

        public string DefaultResult
        {
            get { return this.m_DefaultResult; }
            set
            {
                if (this.m_DefaultResult != value)
                {
                    this.m_DefaultResult = value;
                    this.NotifyPropertyChanged("DefaultResult");
                }
            }
        }

        public string HistologyDisplayString
        {
            get { return this.m_HistologyDisplayString; }
            set
            {
                if (this.m_HistologyDisplayString != value)
                {
                    this.m_HistologyDisplayString = value;
                    this.NotifyPropertyChanged("HistologyDisplayString");
                }
            }
        }

        public string StainerType
        {
            get { return this.m_StainerType; }
            set
            {
                if (this.m_StainerType != value)
                {
                    this.m_StainerType = value;
                    this.NotifyPropertyChanged("StainerType");
                }
            }
        }

        public string VentanaBenchMarkProtocolName
        {
            get { return this.m_VentanaBenchMarkProtocolName; }
            set
            {
                if (this.m_VentanaBenchMarkProtocolName != value)
                {
                    this.m_VentanaBenchMarkProtocolName = value;
                    this.NotifyPropertyChanged("VentanaBenchMarkProtocolName");
                }
            }
        }

        public string CPTCode
        {
            get { return this.m_CPTCode; }
            set
            {
                if (this.m_CPTCode != value)
                {
                    this.m_CPTCode = value;
                    this.NotifyPropertyChanged("CPTCode");
                }
            }
        }

        public string SubsequentCPTCode
        {
            get { return this.m_SubsequentCPTCode; }
            set
            {
                if (this.m_SubsequentCPTCode != value)
                {
                    this.m_SubsequentCPTCode = value;
                    this.NotifyPropertyChanged("SubsequentCPTCode");
                }
            }
        }

        public string GCode
        {
            get { return this.m_GCode; }
            set
            {
                if (this.m_GCode != value)
                {
                    this.m_GCode = value;
                    this.NotifyPropertyChanged("GCode");
                }
            }
        }

        public int VentanaBenchMarkId
        {
            get { return this.m_VentanaBenchMarkId; }
            set
            {
                if (this.m_VentanaBenchMarkId != value)
                {
                    this.m_VentanaBenchMarkId = value;
                    this.NotifyPropertyChanged("VentanaBenchMarkId");
                }
            }
        }

        public int? VentanaBenchMarkWetId
        {
            get { return this.m_VentanaBenchMarkWetId; }
            set
            {
                if (this.m_VentanaBenchMarkWetId != value)
                {
                    this.m_VentanaBenchMarkWetId = value;
                    this.NotifyPropertyChanged("VentanaBenchMarkWetId");
                }
            }
        }

        public int StainResultGroupId
        {
            get { return this.m_StainResultGroupId; }
            set
            {
                if (this.m_StainResultGroupId != value)
                {
                    this.m_StainResultGroupId = value;
                    this.NotifyPropertyChanged("StainResultGroupId");
                }
            }
        }

        public bool IsBillable
        {
            get { return this.m_IsBillable; }
            set
            {
                if (this.m_IsBillable != value)
                {
                    this.m_IsBillable = value;
                    this.NotifyPropertyChanged("IsBillable");
                }
            }
        }

        public bool HasGCode
        {
            get { return this.m_HasGCode; }
            set
            {
                if (this.m_HasGCode != value)
                {
                    this.m_HasGCode = value;
                    this.NotifyPropertyChanged("HasGCode");
                }
            }
        }

        public bool IsDualOrder
        {
            get { return this.m_IsDualOrder; }
            set
            {
                if (this.m_IsDualOrder != value)
                {
                    this.m_IsDualOrder = value;
                    this.NotifyPropertyChanged("IsDualOrder");
                }
            }
        }

        public bool HasCptCodeLevels
        {
            get { return this.m_HasCptCodeLevels; }
            set
            {
                if (this.m_HasCptCodeLevels != value)
                {
                    this.m_HasCptCodeLevels = value;
                    this.NotifyPropertyChanged("HasCptCodeLevels");
                }
            }
        }

        public bool Active
        {
            get { return this.m_Active; }
            set
            {
                if (this.m_Active != value)
                {
                    this.m_Active = value;
                    this.NotifyPropertyChanged("Active");
                }
            }
        }

        public bool NeedsAcknowledgement
        {
            get { return this.m_NeedsAcknowledgement; }
            set
            {
                if (this.m_NeedsAcknowledgement != value)
                {
                    this.m_NeedsAcknowledgement = value;
                    this.NotifyPropertyChanged("NeedsAcknowledgement");
                }
            }
        }

        public bool UseWetProtocol
        {
            get { return this.m_UseWetProtocol; }
            set
            {
                if (this.m_UseWetProtocol != value)
                {
                    this.m_UseWetProtocol = value;
                    this.NotifyPropertyChanged("UseWetProtocol");
                }
            }
        }

        public bool PerformedByHand
        {
            get { return this.m_PerformedByHand; }
            set
            {
                if (this.m_PerformedByHand != value)
                {
                    this.m_PerformedByHand = value;
                    this.NotifyPropertyChanged("PerformedByHand");
                }
            }
        }

        public bool RequestForAdditionalReport
        {
            get { return this.m_RequestForAdditionalReport; }
            set
            {
                if (this.m_RequestForAdditionalReport != value)
                {
                    this.m_RequestForAdditionalReport = value;
                    this.NotifyPropertyChanged("RequestForAdditionalReport");
                }
            }
        }

        public bool HasWetProtocol
        {
            get { return this.m_HasWetProtocol; }
            set
            {
                if (this.m_HasWetProtocol != value)
                {
                    this.m_HasWetProtocol = value;
                    this.NotifyPropertyChanged("HasWetProtocol");
                }
            }
        }




        public Stain FirstStain
        {
            get { return this.m_FirstStain; }
            set
            {
                if (this.m_FirstStain != value)
                {
                    this.m_FirstStain = value;
                    this.NotifyPropertyChanged("FirstStain");
                }
            }
        }

        public Stain SecondStain
        {
            get { return this.m_SecondStain; }
            set
            {
                if (this.m_SecondStain != value)
                {
                    this.m_SecondStain = value;
                    this.NotifyPropertyChanged("SecondStain");
                }
            }
        }

        public string DepricatedFirstTestId
        {
            get { return this.m_DepricatedFirstTestId; }
            set
            {
                if (this.m_DepricatedFirstTestId != value)
                {
                    this.m_DepricatedFirstTestId = value;
                    this.NotifyPropertyChanged("DepricatedFirstTestId");
                }
            }
        }

        public string DepricatedSecondTestId
        {
            get { return this.m_DepricatedSecondTestId; }
            set
            {
                if (this.m_DepricatedSecondTestId != value)
                {
                    this.m_DepricatedSecondTestId = value;
                    this.NotifyPropertyChanged("DepricatedSecondTestId");
                }
            }
        }

    }
}
