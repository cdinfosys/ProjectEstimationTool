using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEstimationTool.ViewModels;

namespace ProjectEstimationTool.Classes
{
    public class DisableMainWindowStep : ProjectModelProcessingStepBase
    {
        private MainWindowViewModel mMainWindowViewModel;
        private Boolean mMainWindowDisabled;

        public DisableMainWindowStep(MainWindowViewModel mainWindowViewModel, ProjectModelProcessingStepBase nextStep)
            :   base(nextStep)
        {
            this.mMainWindowViewModel = mainWindowViewModel;
        }

        public DisableMainWindowStep(MainWindowViewModel mainWindowViewModel)
        {
            this.mMainWindowViewModel = mainWindowViewModel;
        }

        protected override Boolean PerformAction()
        {
            mMainWindowDisabled = mMainWindowViewModel.IsMainWindowDisabled;
            mMainWindowViewModel.IsMainWindowDisabled = true;

            return true;
        }

        protected override void PerformCleanup()
        {
            mMainWindowViewModel.IsMainWindowDisabled = mMainWindowDisabled;
        }
    }
}
