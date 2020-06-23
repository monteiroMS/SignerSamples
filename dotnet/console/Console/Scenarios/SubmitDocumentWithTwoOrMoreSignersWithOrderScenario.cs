﻿using Lacuna.Signer.Api;
using Lacuna.Signer.Api.Documents;
using Lacuna.Signer.Api.FlowActions;
using Lacuna.Signer.Api.Users;
using System;
using System.Collections.Generic;
using System.IO;

namespace Console.Scenarios
{
    public class SubmitDocumentWithTwoOrMoreSignersWithOrderScenario : Scenario
    {
        /**
         * This scenario shows step-by-step the submission of a document
         * to the signer instance where there are two participant in the role
         * of signatories and the there's a required order for the signatures.
         */
        public override void Run()
        {
            // 1. The file's bytes must be read by the application and uploaded using the method UploadFileAsync.
            var filePath = "sample.pdf";
            var fileName = Path.GetFileName(filePath);
            var file = File.ReadAllBytes(filePath);
            var uploadModel = signerClient.UploadFileAsync(fileName, file, "application/pdf");

            // 2. Signer's server expects a FileUploadModel's list to create a document.
            var fileUploadModel = new FileUploadModel(uploadModel.Result) { DisplayName = "Two Signers With Order " + DateTime.UtcNow.ToString() };
            var fileUploadModelList = new List<FileUploadModel>() { fileUploadModel };

            // 3. Foreach participant on the flow, you'll need to create an instance of ParticipantUserModel.
            var participantUserOne = new ParticipantUserModel()
            {
                Name = "Jack Bauer",
                Email = "jack.bauer@mailinator.com",
                Identifier = "75502846369"
            };

            var participantUserTwo = new ParticipantUserModel()
            {
                Name = "James Bond",
                Email = "james.bond@mailinator.com",
                Identifier = "95588148061"
            };

            // 4. You'll need to create a FlowActionCreateModel's instance foreach ParticipantUserModel
            //    created in the previous step. The FlowActionCreateModel is responsible for holding
            //    the personal data of the participant and the type of action that it will peform on the flow.
            //    In the case of order for the flow actions it's necessary to assign values to the `Step` propertie
            //    of the instances, smaller numbers represents action that comes first.
            var flowActionCreateModelOne = new FlowActionCreateModel()
            {
                Type = FlowActionType.Signer,
                User = participantUserOne,
                Step = 1
            };

            var flowActionCreateModelTwo = new FlowActionCreateModel()
            {
                Type = FlowActionType.Signer,
                User = participantUserTwo,
                Step = 2
            };

            // 5. Signer's server expects a FlowActionCreateModel's list to create a document.
            var flowActionCreateModelList = new List<FlowActionCreateModel>() {
                flowActionCreateModelOne,
                flowActionCreateModelTwo
            };

            // 6. To create the document request, use the list of FileUploadModel and the list of FlowActionCreateModel.
            var documentRequest = new CreateDocumentRequest()
            {
                Files = fileUploadModelList,
                FlowActions = flowActionCreateModelList
            };
            var documentResults = signerClient.CreateDocumentAsync(documentRequest);

            // 7. To notify the participant:
            foreach (var documentResult in documentResults.Result)
            {
                // 7.1. Extract the information of the flow using the following procedure.
                var details = signerClient.GetDocumentDetailsAsync(documentResult.DocumentId);
                foreach (var flowAction in details.Result.FlowActions)
                {
                    // 7.2. Send notification to the participant.
                    _ = signerClient.SendFlowActionReminderAsync(documentResult.DocumentId, flowAction.Id);
                }
            }
        }
    }
}
