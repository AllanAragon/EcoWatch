using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EcoWatch.ViewModels
{
	public partial class IssueViewModel : ViewModelBase
	{ 
		public IssueViewModel()
		{
			EnableAddingSolution = false;
		}
	}
}

