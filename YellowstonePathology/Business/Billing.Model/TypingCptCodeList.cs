using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace YellowstonePathology.Business.Billing.Model
{
	public class TypingCptCodeList : ObservableCollection<TypingCptCodeListItem>
	{
		public TypingCptCodeList()
		{
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("85060", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("85097", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88300", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88302", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88304", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88305", null)));   
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88305", "26")));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88307", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88309", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88104", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88112", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88160", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88161", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88172", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88173", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88177", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88311", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88321", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88323", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88325", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88329", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88331", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88332", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88333", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88334", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88342", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("88363", null)));
            this.Add(new TypingCptCodeListItem(CptCodeCollection.Get("99000", null)));
        }
    }
}
