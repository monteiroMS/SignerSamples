﻿using Lacuna.Signer.Api;
using Lacuna.Signer.Api.Documents;
using Lacuna.Signer.Api.FlowActions;
using Lacuna.Signer.Api.Users;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Console.Scenarios
{
    public class EmbeddedSignatureScenario : Scenario
    {
        /**
         * This scenario demonstrates the creation of a document
         * and the generation of an action URL for the embedded signature 
         * integration.
         */
        public override async Task RunAsync()
        {
            // 1. The file's bytes must be read by the application and uploaded
            var filePath = "sample.pdf";
            var fileName = Path.GetFileName(filePath);
            var file = File.ReadAllBytes(filePath);
            var uploadModel = await SignerClient.UploadFileAsync(fileName, file, "application/pdf");

            // 2. Define the name of the document which will be visible in the application
            var fileUploadModel = new FileUploadModel(uploadModel) { DisplayName = "Embedded Signature Sample" };

            // 3. For each participant on the flow, create one instance of ParticipantUserModel
            var participantUser = new ParticipantUserModel()
            {
                Name = "Jack Bauer",
                Email = "jack.bauer@mailinator.com",
                Identifier = "75502846369"
            };

            // 4. Create a FlowActionCreateModel instance for each action (signature or approval) in the flow.
            //    This object is responsible for defining the personal data of the participant and the type of 
            //    action that he will peform on the flow
            var flowActionCreateModel = new FlowActionCreateModel()
            {
                Type = FlowActionType.Signer,
                User = participantUser
            };

            // 5. Send the document create request
            var documentRequest = new CreateDocumentRequest()
            {
                Files = new List<FileUploadModel>() { fileUploadModel },
                FlowActions = new List<FlowActionCreateModel>() { flowActionCreateModel }
            };
            var result = (await SignerClient.CreateDocumentAsync(documentRequest)).First();

            // 6. Get the embed URL for the participant
            var actionUrlRequest = new ActionUrlRequest()
            {
                Identifier = participantUser.Identifier
            };
            var actionUrlResponse = await SignerClient.GetActionUrlAsync(result.DocumentId, actionUrlRequest);

            // 7. Load the embed URL in your own application using the LacunaSignerWidget as described in 
            //    https://docs.lacunasoftware.com/pt-br/articles/signer/embedded-signature.html
            System.Console.WriteLine(actionUrlResponse.EmbedUrl);
        }
    }
}
