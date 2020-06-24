﻿using Lacuna.Signer.Api;
using Lacuna.Signer.Api.Documents;
using Lacuna.Signer.Api.FlowActions;
using Lacuna.Signer.Api.Users;
using Lacuna.Signer.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Console.Scenarios
{
    public abstract class Scenario
    {
        protected SignerClient signerClient;

        public Scenario()
        {
            // Homologation instance
            var domain = "https://signer-lac.azurewebsites.net";
            // Application credentials token.
            var token = "API Sample App|43fc0da834e48b4b840fd6e8c37196cf29f919e5daedba0f1a5ec17406c13a99";
            signerClient = new SignerClient(domain, token);
        }

        public abstract Task RunAsync();

        protected async Task<CreateDocumentResult> createDocumentAsync()
        {
            var filePath = "sample.pdf";
            var fileName = Path.GetFileName(filePath);
            var file = File.ReadAllBytes(filePath);
            var uploadModel = await signerClient.UploadFileAsync(fileName, file, "application/pdf");

            var documentModel = new FileUploadModel(uploadModel) { DisplayName = "Check Status Sample" };

            var participant = new ParticipantUserModel()
            {
                Name = "Jack Bauer",
                Email = "jack.bauer@mailinator.com",
                Identifier = "75502846369"
            };

            var flowAction = new FlowActionCreateModel()
            {
                Type = FlowActionType.Signer,
                User = participant
            };

            var documentRequest = new CreateDocumentRequest()
            {
                Files = new List<FileUploadModel>() { documentModel },
                FlowActions = new List<FlowActionCreateModel>() { flowAction }
            };

            return (await signerClient.CreateDocumentAsync(documentRequest)).First();
        }
    }
}
