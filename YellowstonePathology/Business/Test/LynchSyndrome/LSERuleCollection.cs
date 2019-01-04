﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace YellowstonePathology.Business.Test.LynchSyndrome
{
	public class LSERuleCollection : ObservableCollection<LSERule>
	{
		public LSERuleCollection()
		{
			
		}

        public LSERuleCollection OrderByMatched()
        {
            LSERuleCollection result = new LSERuleCollection();
            IOrderedEnumerable<LSERule> orderedResult= this.OrderBy(i => i.IHCMatched);
            for(int i=orderedResult.Count() - 1; i>=0; i--)
            {
                result.Add(orderedResult.ElementAt(i));
            }
            return result;
        }

        public void ClearMatched()
        {
            foreach (LSERule lseResult in this)
            {
                lseResult.IHCMatched = false;
            }
        }

        public bool HasIHCMatch(LSERule lseResultToMatch)
        {
            bool result = false;
            foreach(LSERule lseResult in this)
            {
                if(lseResult.IsIHCMatch(lseResultToMatch) == true)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void SetIHCMatch(LSERule lseResultToMatch)
        {            
            foreach (LSERule lseResult in this)
            {
                if (lseResult.IsIHCMatch(lseResultToMatch) == true)
                {
                    lseResult.IHCMatched = true;                    
                }
            }            
        }

        public static LSERuleCollection GetAll()
        {
            LSERuleCollection result = new LSERuleCollection();

            result.Add(new LSEColonAllIntact());
            result.Add(new LSEColonMSH2MSH6Loss());
            result.Add(new LSEColonMLH1Loss());
            result.Add(new LSEColonMLH1LossBRAFD());
            result.Add(new LSEColonMSH2Loss());
            result.Add(new LSEColonMSH6Loss());
            result.Add(new LSEColonPMS2Loss());            

            result.Add(new LSEGYNResult1());
            result.Add(new LSEGYNResult2());
            result.Add(new LSEGYNResult3());
            result.Add(new LSEGYNResult3a());

            result.Add(new LSEGeneralResult1());
            result.Add(new LSEGeneralResult2());

            return result;
        }

        public static LSERuleCollection GetColonResults()
        {
            LSERuleCollection result = new LSERuleCollection();

            result.Add(new LSEColonAllIntact());
            result.Add(new LSEColonMLH1Loss());
            result.Add(new LSEColonMLH1LossBRAFD());
            result.Add(new LSEColonMSH2MSH6Loss());
            result.Add(new LSEColonMSH2Loss());
            result.Add(new LSEColonMSH6Loss());
            result.Add(new LSEColonPMS2Loss());            

            return result;
        }

        public static LSERuleCollection GetGYNResults()
        {
            LSERuleCollection result = new LSERuleCollection();
            result.Add(new LSEGYNResult1());
            result.Add(new LSEGYNResult2());
            result.Add(new LSEGYNResult3());
            result.Add(new LSEGYNResult3a());
            return result;
        }

        public static LSERuleCollection GetProstateResults()
        {
            LSERuleCollection result = new LSERuleCollection();
            result.Add(new LSEGeneralResult1());
            result.Add(new LSEGeneralResult2());
            return result;
        }        

        /*
        public static LSEResult GetResult(LSEResult evalResult, string lseType)
		{
			LSEResult result = null;
            LSEResultCollection collection = null;

            if (lseType == YellowstonePathology.Business.Test.LynchSyndrome.LSEType.COLON)
            {
                collection = LSEResultCollection.GetColonResults();
            }
            else if (lseType == YellowstonePathology.Business.Test.LynchSyndrome.LSEType.GYN)
            {
                collection = LSEResultCollection.GetGYNResults();
            }
            else if (lseType == YellowstonePathology.Business.Test.LynchSyndrome.LSEType.GENERAL)
            {
                collection = LSEResultCollection.GetProstateResults();
            }
            else if (lseType == YellowstonePathology.Business.Test.LynchSyndrome.LSEType.NOTSET)
            {
                collection = new LSEResultCollection();
            }

            if (lseType == YellowstonePathology.Business.Test.LynchSyndrome.LSEType.GENERAL)
            {
                if(evalResult.AreAllIntact() == true)
                {
                    result = new Business.Test.LynchSyndrome.LSEGeneralResult1();
                }
                else if(evalResult.AreAnyLoss() == true)
                {
                    result = new Business.Test.LynchSyndrome.LSEGeneralResult2();
                }

                result.MLH1Result = evalResult.MLH1Result;
                result.MSH2Result = evalResult.MSH2Result;
                result.MSH6Result = evalResult.MSH6Result;
                result.PMS2Result = evalResult.PMS2Result;
            }
            else
            {
                foreach (LSEResult lSEResult in collection)
                {
                    if (lSEResult.MLH1Result == evalResult.MLH1Result &&
                        lSEResult.MSH2Result == evalResult.MSH2Result &&
                        lSEResult.MSH6Result == evalResult.MSH6Result &&
                        lSEResult.PMS2Result == evalResult.PMS2Result)
                    {
                        bool brafResultIsEqual = false;
                        if (evalResult.ReflexToBRAFMeth == true)
                        {
                            brafResultIsEqual = (lSEResult.BrafResult == evalResult.BrafResult);
                        }
                        else
                        {
                            brafResultIsEqual = true;
                        }

                        bool methResultIsEqual = false;
                        if (lSEResult.MethResult == LSEResultEnum.NotPerformed)
                        {
                            methResultIsEqual = true;
                        }
                        else
                        {
                            methResultIsEqual = (lSEResult.MethResult == evalResult.MethResult);
                        }

                        if (brafResultIsEqual == true && methResultIsEqual == true)
                        {
                            result = lSEResult;
                            break;
                        }
                    }
                }
            }                          			            

			return result;
		}
        */
	}
}
