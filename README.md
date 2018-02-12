[![Project Status](http://opensource.box.com/badges/active.svg)](http://opensource.box.com/badges)

eSign for Acumatica Cloud ERP
==================================
This is a unified eSignature integration for Acumatica Cloud ERP that supports DocuSign and Adobe Sign.

With the eSign integration for Acumatica Cloud ERP, users can:
-	Create envelopes and prepare documents for signing
-	Send documents for signing and manage documents
-	Check status of document sent for signing
-	Send documents from CRM, Financials, Distribution and other modules using Document Management

Note: Accounts from providers are required to use this integration.

### DocuSign
DocuSign provides a complete solution to send, sign and manage documents accelerating the time from lead to deal. With DocuSign, users can quickly and securely review and sign documents.
With the integration in Acumatica, users can now take advantage of the DocuSign capability from within Acumatica Cloud ERP. Users working on a sales order or an SOW or a contract in an opportunity, can now send it for signing from within Acumatica’s Document management module.

For pricing please visit: [DocuSign Pricing](https://www.docusign.com/b/products-and-pricing)

To get Integrator Key, Please visit: [DocuSign Integrator Key](https://www.docusign.com/developer-center/api-overview#go-live)

### Adobe Sign
Adobe Sign, a solution from the trusted digital document leader, makes it simple to collect legal electronic and digital signatures. And it’s just as easy to sign as it is to send — on any device. And it complies with the broadest range of legal requirements, the most demanding industry regulations, and the most stringent security standards around the world.

For pricing please visit: [AdobeSign Pricing](https://acrobat.adobe.com/us/en/sign/pricing/plans.html)

### Prerequisites
* Acumatica 2017 R2 or higher

Getting Started
-----------

### Install eSign Customization Project

1. Download AcumaticaESignIntegration.zip from [Acumatica Partner Portal](https://portal.acumatica.com/downloads/esign-integration-download-2/)
2. In your Acumatica ERP instance, import this as a customization project
3. Publish the customization project

![Screenshot](/_ReadMeImages/Image1.png)

Next step is to configure the eSign provider accounts. Follow the instructions below:

### Configure Adobe Account

First step in Adobe Sign configuration is to allow Acumatica Cloud ERP and Adobe Sign to have access. This step is usually performed by your application admin.

1. Open Adobe [Sign](https://secure.na2.echosign.com)
2. Login to your Adobe Sign account by clicking Sign in and entering your account credentials
3. Once logged in, select “Account” from the menu and select “Adobe API”-> “API Applications” from the left navigation as show below:

   ![Screenshot](/_ReadMeImages/Image2a0.png)

4. In the “API Application” screen, hit create and provide the following information about the app:

   ![Screenshot](/_ReadMeImages/Image2a1.png)

   •  Name: Type a name for your Acumatica ERP instance.
   
   •  Display Name: Provide a descriptive name for your Acumatica ERP instance.
   
   •  Domain: Select “Customer”

   Save this information and you will see a new application created in the API Application list.

5. Select the newly created application and choose “Configure OAuth for Application” from the menu. In the Configure OAuth, specify the following information:

   ![Screenshot](/_ReadMeImages/Image2a2.png)

   •	Redirect URL: Enter the link to your Acumatica ERP site in the following format: 
   
      https://[full URL of your Acumatica ERP site]/Pages/ES/ESign.aspx (for example, https://app.site.net/Pages/ES/ESign.aspx).
      
   •	Enable the following scopes:
      
      1. user_login
      2. agreement_read
      3. agreement_write
      4. agreement_sent
      5. workflow_read
      6. workflow_write
      
   •	Set all modifiers to “Account”

Next step is to configure the Adobe Sign user account using eSign Account screen.

   ![Screenshot](/_ReadMeImages/Image2a3.png)


6. Provide a name for this Adobe Sign Account (e.g. MYADOBESIGN)

   ![Screenshot](/_ReadMeImages/Image2a4.png)

7. Select “AdobeSign” as Provider Type
8. Select either “Individual” or “Shared” (Team/Enterprise) account type
9. Assign the account to Acumatica user (This user can send documents for signing using the Adobe Sign account)
10. Provide the Adobe Sign API URL (e.g. https://secure.na2.echosign.com). 
11. Specify the Client ID and Client Secret from Adobe Sign
12. Click “CONNECT” to establish connection between Acumatica Cloud ERP and Adobe Sign. This will popup a window (as show below) to allow access between these two applications. Once allowed, your Adobe Sign account is ready to use.

   ![Screenshot](/_ReadMeImages/Image2a5.png)

NOTE: Check this [guide](https://helpx.adobe.com/sign/help/quick-setup-guide.html) from Adobe Sign for more info on how to add new users to your team or group account. 

### Configure DocuSign Account

Next step is to configure the Adobe Sign user account using eSign Account screen.

 ![Screenshot](/_ReadMeImages/Image2.png)

1. Provide a name for this Docusign Account (e.g. MYDOCUSIGN).
2. Select either “Individual” or “Shared” (Corporate) account type
3. Assign the account to Acumatica user (This user can send documents for signing using the Docusign account)
4. Specify the Email and Password for the Docusign account (If you don’t have an DocuSign account, create one first at [DocuSign](http://www.docusign.com))
5. Click “Connect” to verify the configuration
6. If you are using sandbox/demo/developer account, you need to check "Use Test API" and specify https://demo.docusign.net/restapi as API URL.

###	Prepare and Send Document
With the configuration completed, user can now send documents for signing from within Acumatica. This can be initiated from any Acumatica module (Financials, CRM, Distribution) where files are handled. (See Image below)

  ![Screenshot](/_ReadMeImages/Image3.png)

Clicking Edit in the above window, launches the “File Maintenance” dialog with the “ESIGN” option.

  ![Screenshot](/_ReadMeImages/Image4.png)

Click on “ESIGN” to prepare the envelope. In this dialog, specify subject and message and specify the receipients to sign the document and the ones to receive copy. You can manually add the receipents or use the contact selector to pick a contact from Acumatica. 
If the underlying entity had a contact specified (e.g. Contact in Opportunity), that contact will be loaded automatically.

  ![Screenshot](/_ReadMeImages/Image5.png)

Click “SEND” to get the document over to the provider (Adobe Sign or DocuSign) for signing. This will popup a provider screen to place tags (e.g. name, company info, signature) and to send it for signing.
Once the document is sent for signing, the status will be updated in the “File Maintenance” dialog. 

  ![Screenshot](/_ReadMeImages/Image6.png)

### Checking Document Status
User can check the status the documents in the new “eSign Central”. Here you will see list of all documents that were sent and with the status. See image below for an example.

  ![Screenshot](/_ReadMeImages/Image7.png)

Users can check the status of documents either manually or by scheduling a synch from the “eSign Sync” screen. In this screen, users can manually get the status update by either using “PROCESS” or “PROCESS ALL” options. This action will perform a synch with the provider and retrieve latest status information for the documents in the queue.
Alternatively, users can setup a synch frequency using the  "Schedules" option to automaticaly retrieve status at specified schedule. 

  ![Screenshot](/_ReadMeImages/Image8.png)

### DocuSign Demo
  [![Screenshot](/_ReadMeImages/Image9.png)](https://www.youtube.com/watch?v=Mv-b8_iwLiE&feature=youtu.be)

### Adobe Sign Demo
  [![Screenshot](/_ReadMeImages/Image10.png)](https://fast.wistia.net/embed/iframe/tg7wt30y9o#?secret=uHsccqChMm)


Known Issues
------------
None at the moment.

## Copyright and License

Copyright © `2017` `Acumatica`

This component is licensed under the MIT License, a copy of which is available online [here](LICENSE.md)
