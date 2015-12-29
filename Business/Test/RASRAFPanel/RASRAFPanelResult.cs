﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowstonePathology.Business.Test.RASRAFPanel
{
    public class RASRAFPanelResult
    {
    	public static string Method = "Nucleic acid was isolated from cells or microdissection-enriched FFPE tissue. The RAS/RAF Profile includes massive " +
    		"parallel sequencing and mutation analysis of the following genes: BRAF: EX: 11 and 15 (AA: G2695_K2727 and D2725_K2756); HRAS: EX: 2 and 3 (AA: " +
    		"M1_G15 and D38_E62); KRAS: EX: 2, 3 and 4 (AA: M1_I21, D38_E62, and K104_A146); and NRAS: EX: 2, 3 and 4 (AA: M1_A18, D38_E62, and N104_A146). " +
    		"When applicable, confirmation of the presence or lack of mutation was performed using Sanger sequencing on the following genes: BRAF, HRAS, KRAS, " +
    		"and NRAS. This sequencing method has a typical sensitivity of 5% for detecting mutations. Various factors including quantity and quality of nucleic " +
    		"acid, sample preparation and sample age can affect assay performance.";
        public static string References = "1.  Peeters M, Douillard J, Van Cutsem E, Siena S, Zhang K, Williams R, Wiezorek J Mutant KRAS codon 12 and 13 " +
        	"alleles in patients with metastatic colorectal cancer: assessment as prognostic and predictive biomarkers of response to panitumumab." + Environment.NewLine +
        	"2.  Bokemeyer C, Van Cutsem E, Rougier P, Ciardiello F, Heeger S, Schlichting M, Celik I, Köhne C Addition of cetuximab to chemotherapy as first-line" +
        	"treatment for KRAS wild-type metastatic colorectal cancer: pooled analysis of the CRYSTAL and OPUS randomised clinical trials." + Environment.NewLine +
        	"3.  Di Nicolantonio F, Martini M, Molinari F, Sartore-Bianchi A, Arena S, Saletti P, De Dosso S, Mazzucchelli L, Frattini M, Siena S, Bardelli A " +
        	"Wild-type BRAF is required for response to panitumumab or cetuximab in metastatic colorectal cancer." + Environment.NewLine +
        	"4.  De Roock W, Claes B, Bernasconi D, De Schutter J, Biesmans B, Fountzilas G, Kalogeras K, Kotoula V, Papamichael D, Laurent-Puig P, Penault-Llorca " +
        	"F, Rougier P, Vincenzi B, Santini D, Tonini G, CappuzzoF, Frattini M, Molinari F, Saletti P, De Dosso S, Martini M, Bardelli A, Siena S, " +
        	"Sartore-Bianchi A, Tabernero J, Macarulla T, Di Fiore F, Gangloff A, Ciardiello F, Pfeiffer P, Qvortrup C, Hansen T, Van Cutsem E, Piessevaux H, " +
        	"Lambrechts D, Delorenzi M, Tejpar S Effects of KRAS, BRAF, NRAS, and PIK3CA mutations on the efficacy of cetuximab plus chemotherapy in " +
        	"chemotherapy-refractory metastatic colorectal cancer: a retrospective consortium analysis.";
        public static string Detected = "Detected";
        public static string NotDetected = "Not Detected";
        public static string NA = "N/A";
        
        protected const string m_Interpretation = "KRAS, NRAS, HRAS, and BRAF are members of the RAS/RAF/MAPK pathway. Current guidelines require KRAS and NRAS " +
        	"testing in metastatic colorectal cancer for determination of anti-EGFR therapy, and recommend BRAF testing as a marker of poor prognosis.  Patients " +
        	"with any known KRAS mutation (exon 2 or non-exon 2) or NRAS should not be treated with either cetuximab or panitumumab per NCCN Guidelines. BRAF " +
        	"mutations may predict lack of response to anti-EGFR therapy; evidence is mixed.";
        protected const string m_DetectedInterpretation = "-THERE ARE MUTATIONS IN THE ";
        protected const string m_NotDetectedInterpretation = "NO EVIDENCE OF MUTATION IN ";
        protected const string m_NotDetectedInterpretationEnding = "-THE LACK OF MUTATIONS IN THESE GENES IN A COLORECTAL TUMORS SUGGESTS SENSITIVITY TO ANTI-EGFR ANTIBODIES.";
        protected const string m_TestDevelopment = "The performance characteristics of this test have been determined by NeoGenomics Laboratories. This test has " +
            "not been approved by the FDA. The FDA has determined such clearance or approval is not necessary. This laboratory is CLIA certified to perform high " +
            "complexity clinical testing.";

        public RASRAFPanelResult()
        {

        }

        public void SetResults(RASRAFPanelTestOrder testOrder)
        {
        	List<string> detectedMutations = new List<string>();
        	List<string> notDetectedMutations = new List<string>();
        	
        	if(testOrder.BRAFResult == Detected) detectedMutations.Add("BRAF");
    		else notDetectedMutations.Add("BRAF");
    		
        	if(testOrder.KRASResult == Detected) detectedMutations.Add("KRAS");
    		else notDetectedMutations.Add("KRAS");
    		
        	if(testOrder.NRASResult == Detected) detectedMutations.Add("NRAS");
    		else notDetectedMutations.Add("NRAS");
    		
        	if(testOrder.HRASResult == Detected) detectedMutations.Add("HRAS");
    		else notDetectedMutations.Add("HRAS");    		
    		
        	StringBuilder detectedInterpretation = new StringBuilder();
        	if(detectedMutations.Count > 0)
        	{
        		detectedInterpretation.Append(m_DetectedInterpretation);
        		for(int idx = 0; idx < detectedMutations.Count; idx++)
        		{
        			detectedInterpretation.Append(detectedMutations[idx]);
        			if(idx == detectedMutations.Count - 2) detectedInterpretation.Append(" AND ");
        			else if(idx <  detectedMutations.Count - 2) detectedInterpretation.Append(", ");
        		}
        		
        		if(notDetectedMutations.Count == 0) detectedInterpretation.Append(" GENES.");
        		else detectedInterpretation.Append(", ");
        	}
    		
        	StringBuilder notDetectedInterpretation = new StringBuilder();
        	if(notDetectedMutations.Count > 0)
        	{
        		if(detectedMutations.Count > 0) notDetectedInterpretation.Append("BUT ");
        		else notDetectedInterpretation.Append("-");
        		
        		notDetectedInterpretation.Append(m_NotDetectedInterpretation);
        		for(int idx = 0; idx < notDetectedMutations.Count; idx++)
        		{
        			notDetectedInterpretation.Append(notDetectedMutations[idx]);
        			if(idx == notDetectedMutations.Count - 2) notDetectedInterpretation.Append(" AND ");
        			else if(idx <  notDetectedMutations.Count - 2) notDetectedInterpretation.Append(", ");
        		}
        		notDetectedInterpretation.AppendLine(" GENES.");
        		notDetectedInterpretation.AppendLine();
    			notDetectedInterpretation.Append(m_NotDetectedInterpretationEnding);
        	}
        	
        	StringBuilder interpretation = new StringBuilder();
        	interpretation.Append(detectedInterpretation.ToString());
        	interpretation.Append(notDetectedInterpretation.ToString());
        	interpretation.AppendLine();
        	interpretation.AppendLine();
        	interpretation.Append(m_Interpretation);
        	
        	testOrder.Interpretation = interpretation.ToString();

            StringBuilder disclaimer = new StringBuilder();
            disclaimer.AppendLine(testOrder.GetLocationPerformedComment());
            disclaimer.Append(m_TestDevelopment);
            testOrder.ReportDisclaimer = disclaimer.ToString();
        }
        
        public static void SetBRAFDetected(RASRAFPanelTestOrder testOrder)
        {
        	if(testOrder.BRAFAlternateNucleotideMutationName == RASRAFPanelResult.NA) testOrder.BRAFAlternateNucleotideMutationName = null;
        	if(testOrder.BRAFConsequence == RASRAFPanelResult.NA) testOrder.BRAFConsequence = null;
        	if(testOrder.BRAFMutationName == RASRAFPanelResult.NA) testOrder.BRAFMutationName = null;
        	if(testOrder.BRAFPredictedEffectOnProtein == RASRAFPanelResult.NA) testOrder.BRAFPredictedEffectOnProtein = null;
        }
        
        public static void SetBRAFNotDetected(RASRAFPanelTestOrder testOrder)
        {
			testOrder.BRAFAlternateNucleotideMutationName = RASRAFPanelResult.NA;
			testOrder.BRAFConsequence = RASRAFPanelResult.NA;
			testOrder.BRAFMutationName = RASRAFPanelResult.NA;
			testOrder.BRAFPredictedEffectOnProtein = RASRAFPanelResult.NA;
        }
        
        public static void SetKRASDetected(RASRAFPanelTestOrder testOrder)
        {
        	if(testOrder.KRASAlternateNucleotideMutationName == RASRAFPanelResult.NA) testOrder.KRASAlternateNucleotideMutationName = null;
        	if(testOrder.KRASConsequence == RASRAFPanelResult.NA) testOrder.KRASConsequence = null;
        	if(testOrder.KRASMutationName == RASRAFPanelResult.NA) testOrder.KRASMutationName = null;
        	if(testOrder.KRASPredictedEffectOnProtein == RASRAFPanelResult.NA) testOrder.KRASPredictedEffectOnProtein = null;
        }
        
        public static void SetKRASNotDetected(RASRAFPanelTestOrder testOrder)
        {
			testOrder.KRASAlternateNucleotideMutationName = RASRAFPanelResult.NA;
			testOrder.KRASConsequence = RASRAFPanelResult.NA;
			testOrder.KRASMutationName = RASRAFPanelResult.NA;
			testOrder.KRASPredictedEffectOnProtein = RASRAFPanelResult.NA;
        }
        
        public static void SetNRASDetected(RASRAFPanelTestOrder testOrder)
        {
        	if(testOrder.NRASAlternateNucleotideMutationName == RASRAFPanelResult.NA) testOrder.NRASAlternateNucleotideMutationName = null;
        	if(testOrder.NRASConsequence == RASRAFPanelResult.NA) testOrder.NRASConsequence = null;
        	if(testOrder.NRASMutationName == RASRAFPanelResult.NA) testOrder.NRASMutationName = null;
        	if(testOrder.NRASPredictedEffectOnProtein == RASRAFPanelResult.NA) testOrder.NRASPredictedEffectOnProtein = null;
        }
        
        public static void SetNRASNotDetected(RASRAFPanelTestOrder testOrder)
        {
			testOrder.NRASAlternateNucleotideMutationName = RASRAFPanelResult.NA;
			testOrder.NRASConsequence = RASRAFPanelResult.NA;
			testOrder.NRASMutationName = RASRAFPanelResult.NA;
			testOrder.NRASPredictedEffectOnProtein = RASRAFPanelResult.NA;
        }
        
        public static void SetHRASDetected(RASRAFPanelTestOrder testOrder)
        {
        	if(testOrder.HRASAlternateNucleotideMutationName == RASRAFPanelResult.NA) testOrder.HRASAlternateNucleotideMutationName = null;
        	if(testOrder.HRASConsequence == RASRAFPanelResult.NA) testOrder.HRASConsequence = null;
        	if(testOrder.HRASMutationName == RASRAFPanelResult.NA) testOrder.HRASMutationName = null;
        	if(testOrder.HRASPredictedEffectOnProtein == RASRAFPanelResult.NA) testOrder.HRASPredictedEffectOnProtein = null;
        }
        
        public static void SetHRASNotDetected(RASRAFPanelTestOrder testOrder)
        {
			testOrder.HRASAlternateNucleotideMutationName = RASRAFPanelResult.NA;
			testOrder.HRASConsequence = RASRAFPanelResult.NA;
			testOrder.HRASMutationName = RASRAFPanelResult.NA;
			testOrder.HRASPredictedEffectOnProtein = RASRAFPanelResult.NA;
        }
    }
}
