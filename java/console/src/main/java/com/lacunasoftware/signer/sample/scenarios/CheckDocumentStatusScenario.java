package com.lacunasoftware.signer.sample.scenarios;

import java.io.IOException;
import com.lacunasoftware.signer.ActionStatus;
import com.lacunasoftware.signer.CreateDocumentResult;
import com.lacunasoftware.signer.DocumentModel;

import com.lacunasoftware.signer.FlowActionModel;

import com.lacunasoftware.signer.RestException;

public class CheckDocumentStatusScenario extends Scenario {
    /**
    * This scenario demonstrates how to check if a document is concluded 
    * and the status of it's flow actions.
    */
    @Override
    public void Run() throws IOException, RestException {
        CreateDocumentResult result = createDocument();

        // 1. Get the document's details by it's id
        DocumentModel details = signerClient.getDocumentDetails(result.getDocumentId());
        
        // 2. Check if the whole flow is concluded
        if (details.isConcluded()) {

        }

        // 3. If needed, check the status of individual flow actions
        for (FlowActionModel flowAction : details.getFlowActions()) {
            if (flowAction.getStatus() == ActionStatus.COMPLETED) {
                
            }
        }

        /**
        * NOTE: 
        * 
        * The best way to know the exact time a document's flow is concluded is by enabling a webhook in your organization on the
        * application. Whenever the flow of a document is completed, the application will fire a Webhook event by
        * sending a POST request to a registered URL.
        * 
        * You can find below an example of the handling logic of a webhook event.
        * 
        * Access the following link for information on available Webhook events:
        * https://dropsigner.com/swagger
        */
    }
}