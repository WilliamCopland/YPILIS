using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowstonePathology.Business.Persistence;

namespace YellowstonePathology.Business.Test.DeletionsForGlioma1p19q
{
	[PersistentClass("tblDeletionsForGlioma1p19qTestOrder", "tblPanelSetOrder", "YPIDATA")]
	public class DeletionsForGlioma1p19qTestOrder : YellowstonePathology.Business.Test.PanelSetOrder
	{
		private string m_Result;
		private string m_ResultDescription;
		private string m_Interpretation;
		private string m_ProbeSetDetail;
		private string m_NucleiScored;

		public DeletionsForGlioma1p19qTestOrder()
		{
		}

		public DeletionsForGlioma1p19qTestOrder(string masterAccessionNo, string reportNo, string objectId,
			YellowstonePathology.Business.PanelSet.Model.PanelSet panelSet,
			YellowstonePathology.Business.Interface.IOrderTarget orderTarget,
			bool distribute)
			: base(masterAccessionNo, reportNo, objectId, panelSet, orderTarget, distribute)
		{

		}

		[PersistentStringProperty(50)]
		public string Result
		{
			get { return this.m_Result; }
			set
			{
				if (this.m_Result != value)
				{
					this.m_Result = value;
					this.NotifyPropertyChanged("Result");
				}
			}
		}

		[PersistentStringProperty(50)]
		public string ResultDescription
		{
			get { return this.m_ResultDescription; }
			set
			{
				if (this.m_ResultDescription != value)
				{
					this.m_ResultDescription = value;
					this.NotifyPropertyChanged("ResultDescription");
				}
			}
		}

		[PersistentStringProperty(1000)]
		public string Interpretation
		{
			get { return this.m_Interpretation; }
			set
			{
				if (this.m_Interpretation != value)
				{
					this.m_Interpretation = value;
					this.NotifyPropertyChanged("Interpretation");
				}
			}
		}

		[PersistentStringProperty(1000)]
		public string ProbeSetDetail
		{
			get { return this.m_ProbeSetDetail; }
			set
			{
				if (this.m_ProbeSetDetail != value)
				{
					this.m_ProbeSetDetail = value;
					this.NotifyPropertyChanged("ProbeSetDetail");
				}
			}
		}

		[PersistentStringProperty(100)]
		public string NucleiScored
		{
			get { return this.m_NucleiScored; }
			set
			{
				if (this.m_NucleiScored != value)
				{
					this.m_NucleiScored = value;
					this.NotifyPropertyChanged("NucleiScored");
				}
			}
		}

		public override string ToResultString(YellowstonePathology.Business.Test.AccessionOrder accessionOrder)
        {
            StringBuilder result = new StringBuilder();

			string resultString = this.m_Result;
			if (string.IsNullOrEmpty(this.m_ResultDescription) == false) resultString = this.m_ResultDescription;
            result.Append("Result: ");
			result.AppendLine(resultString);
            result.AppendLine();            

            result.AppendLine("Interpretation: " + this.m_Interpretation);
            result.AppendLine();

            return result.ToString();
        }
	}
}
