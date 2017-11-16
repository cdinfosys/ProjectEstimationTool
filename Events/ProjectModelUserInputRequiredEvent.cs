using System;
using System.Windows;
using Prism.Events;
using ProjectEstimationTool.Classes;

namespace ProjectEstimationTool.Events
{
    class ProjectModelUserInputRequiredEventPayload
    {
        private ProjectModelProcessingStepBase mCurrentStep;
        private String mMessageText;
        private String mMessageBoxCaption;
        private MessageBoxButton mButtons;
        private MessageBoxImage mIcon;
        private MessageBoxResult mDefaultButton;

        public ProjectModelUserInputRequiredEventPayload
        (
            ProjectModelProcessingStepBase currentStep,
            String messageText,
            String messageBoxCaption,
            MessageBoxButton buttons,
            MessageBoxImage icon,
            MessageBoxResult defaultButton
        )
        {
            this.mMessageText = messageText;
            this.mMessageBoxCaption = messageBoxCaption;
            this.mButtons = buttons;
            this.mIcon = icon;
            this.mDefaultButton = defaultButton;
            this.mCurrentStep = currentStep;
        }

        public String MessageText => mMessageText;
        public String MessageBoxCaption => mMessageBoxCaption;
        public MessageBoxButton Buttons => mButtons;
        public MessageBoxImage Icon => mIcon;
        public MessageBoxResult DefaultButton => mDefaultButton;
        public ProjectModelProcessingStepBase CurrentStep => mCurrentStep;
    }

    class ProjectModelUserInputRequiredEvent : PubSubEvent<ProjectModelUserInputRequiredEventPayload>
    {
    }
}
