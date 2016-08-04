﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowstonePathology.Business.Test.NonHodgkinsLymphomaFISHPanel
{
	public class NonHodgkinsLymphomaFISHPanelResult
	{
		protected const string References = "http://atlasgeneticsoncology.org";
		protected const string TestDevelopment = "NeoGenomics Laboratories FISH test uses either FDA cleared and/or analyte specific reagent (ASR) probes.  This test " +
			"was developed and its performance characteristics determined by NeoGenomics Laboratories in Irvine CA.  It has not been cleared or approved by the U.S. " +
			"Food and Drug Administration (FDA).  The FDA has determined that such clearance or approval is not necessary.  This test is used for clinical purposes " +
			"and should not be regarded as investigational or for research.  This laboratory is regulated under CLIA '88 as qualified to perform high complexity " +
			"testing.  Interphase FISH does not include examination of the entire chromosomal complement.  Clinically significant anomalies detectable by routine " +
			"banded cytogenetic analysis may still be present.  Consider reflex banded cytogenetic analysis.";

		public NonHodgkinsLymphomaFISHPanelResult()
		{
		}

        public void SetResults(NonHodgkinsLymphomaFISHPanelTestOrder testOrder)
		{
			testOrder.ReportReferences = References;

			StringBuilder disclaimer = new StringBuilder();
			disclaimer.AppendLine(TestDevelopment + Environment.NewLine);
            disclaimer.AppendLine(testOrder.GetLocationPerformedComment());
			testOrder.ReportDisclaimer = disclaimer.ToString();

		}
	}
}
