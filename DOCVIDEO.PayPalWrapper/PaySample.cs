// # Namespaces 
using System;
using System.Collections.Generic;
using System.IO;
using PayPal.AdaptivePayments;
using PayPal.AdaptivePayments.Model;
using System.Net;
using System.Web;
// # Sample for Pay API 
// Use the Pay API operation to transfer funds from a sender's PayPal account to one or more receivers' PayPal accounts. You can use the Pay API operation to make simple payments, chained payments, or parallel payments; these payments can be explicitly approved, preapproved, or implicitly approved.
// This sample code uses AdaptivePayments .NET SDK to make API call. You can
// download the SDK [here](https://github.com/paypal/sdk-packages/tree/gh-pages/adaptivepayments-sdk/dotnet)
namespace DOCVIDEO.PayPalWrapper
{
    public class PaySample
    {

        // # Parallel Payment
        // `Note:
        // For parallel Payment all the above mentioned request parameters in
        // SimplePay() are required, but in receiverList we can have multiple
        // receivers`
        public PayRequest ParallelPaymentOuter(int rate, string payPalId, long id, long appointmentId)
        {
            // Payment operation
            // PayRequest requestPay = Payment();
            RequestEnvelope envelopeRequest = new RequestEnvelope();
            envelopeRequest.errorLanguage = "en_US";

            List<Receiver> receiverLst = new List<Receiver>();

            // Amount to be credited to the receiver's account
            Receiver receiverA = new Receiver(Convert.ToDecimal(rate));

            // A receiver's email address
            receiverA.email = payPalId;

            receiverLst.Add(receiverA);

            // Amount to be credited to the receiver's account
            Receiver receiverB = new Receiver(Convert.ToDecimal("4.00"));

            // A receiver's email address
            receiverB.email = "sanjeev-facilitator@alistechnology.com";
            receiverLst.Add(receiverB);

            ReceiverList receiverList = new ReceiverList(receiverLst);

            PayRequest requestPay = new PayRequest(envelopeRequest, "PAY", System.Configuration.ConfigurationManager.AppSettings["CANCEL_URL"] + "/DoctorInformation/BookYourAppointment/?pstatus=cancel&id=" + id + "&tabvalue=tab1" + "&appointmentId=" + appointmentId + "&pay=" + 0, "USD", receiverList, System.Configuration.ConfigurationManager.AppSettings["RETURN_URL"] + "/DoctorInformation/BookYourAppointment/?pstatus=success&appointmentId=" + appointmentId + "&tabvalue=tab3" + "&id=" + id + "&pay=" + rate);
            //requestPay.senderEmail = "sanjeevsridhar@gmail.com";
            //return requestPay;


            requestPay.receiverList = receiverList;



            // IPN URL
            //  
            // * PayPal Instant Payment Notification is a call back system that is initiated when a transaction is completed        
            // * The transaction related IPN variables will be received on the call back URL specified in the request       
            // * The IPN variables have to be sent back to the PayPal system for validation, upon validation PayPal will send a response string "VERIFIED" or "INVALID"     
            // * PayPal would continuously resend IPN if a wrong IPN is sent        
            requestPay.ipnNotificationUrl = "http://IPNhost";

            return requestPay;
        }
        public PayRequest ParallelPayment(int rate, string payPalId,long id,long appointmentId)
        {
            // Payment operation
            // PayRequest requestPay = Payment();
            RequestEnvelope envelopeRequest = new RequestEnvelope();
            envelopeRequest.errorLanguage = "en_US";

            List<Receiver> receiverLst = new List<Receiver>();

            // Amount to be credited to the receiver's account
            Receiver receiverA = new Receiver(Convert.ToDecimal(rate));

            // A receiver's email address
            receiverA.email = payPalId;

            receiverLst.Add(receiverA);

            // Amount to be credited to the receiver's account
            Receiver receiverB = new Receiver(Convert.ToDecimal("4.00"));

            // A receiver's email address
            receiverB.email = "sanjeev-facilitator@alistechnology.com";
            receiverLst.Add(receiverB);

            ReceiverList receiverList = new ReceiverList(receiverLst);

            PayRequest requestPay = new PayRequest(envelopeRequest, "PAY", System.Configuration.ConfigurationManager.AppSettings["CANCEL_URL"] + "/PatientProfile/BookYourAppointment/?pstatus=cancel&id=" + id + "&tabvalue=tab1" + "&appointmentId=" + appointmentId, "USD", receiverList, System.Configuration.ConfigurationManager.AppSettings["RETURN_URL"] + "/PatientProfile/BookYourAppointment/?pstatus=success&appointmentId=" + appointmentId + "&tabvalue=tab3" + "&id=" + id + "&pay=" + rate);
            //requestPay.senderEmail = "sanjeevsridhar@gmail.com";
            //return requestPay;


            requestPay.receiverList = receiverList;



            // IPN URL
            //  
            // * PayPal Instant Payment Notification is a call back system that is initiated when a transaction is completed        
            // * The transaction related IPN variables will be received on the call back URL specified in the request       
            // * The IPN variables have to be sent back to the PayPal system for validation, upon validation PayPal will send a response string "VERIFIED" or "INVALID"     
            // * PayPal would continuously resend IPN if a wrong IPN is sent        
            requestPay.ipnNotificationUrl = "http://IPNhost";

            return requestPay;
        }

        public PayRequest SubscriptionPayment(int rate, string userId, string paypalId)
        {
            // Payment operation
            // PayRequest requestPay = Payment();
            RequestEnvelope envelopeRequest = new RequestEnvelope();
            envelopeRequest.errorLanguage = "en_US";

            List<Receiver> receiverLst = new List<Receiver>();

            // Amount to be credited to the receiver's account
            Receiver receiverB = new Receiver(Convert.ToDecimal(rate));

            // A receiver's email address
            receiverB.email = "sanjeev-facilitator@alistechnology.com";
            receiverLst.Add(receiverB);

            ReceiverList receiverList = new ReceiverList(receiverLst);

            PayRequest requestPay = new PayRequest(envelopeRequest, "PAY", System.Configuration.ConfigurationManager.AppSettings["CANCEL_URL"] + "/DoctorInformation/PayPalSubscription/?pstatus=cancel&userid=" + userId, "USD", receiverList, System.Configuration.ConfigurationManager.AppSettings["RETURN_URL"] + "/DoctorInformation/PayPalSubscription/?pstatus=success&userId=" + userId + "&pay=" + rate);
            requestPay.senderEmail = paypalId;
            requestPay.receiverList = receiverList;
            //SenderIdentifier sender = new SenderIdentifier();
            //sender.accountId = userId;
            //sender.email = paypalId;
            //requestPay.sender = sender;

            // IPN URL
            //  
            // * PayPal Instant Payment Notification is a call back system that is initiated when a transaction is completed        
            // * The transaction related IPN variables will be received on the call back URL specified in the request       
            // * The IPN variables have to be sent back to the PayPal system for validation, upon validation PayPal will send a response string "VERIFIED" or "INVALID"     
            // * PayPal would continuously resend IPN if a wrong IPN is sent        
            requestPay.ipnNotificationUrl = System.Configuration.ConfigurationManager.AppSettings["IPN_URL"]+ "/DoctorInformation/PayPalIPIN/";

            return requestPay;
        }

        // # Pay API Operations
        // Use the Pay API operations to transfer funds from a sender’s PayPal account to one or more receivers’ PayPal accounts. You can use the Pay API operation to make simple payments, chained payments, or parallel payments; these payments can be explicitly approved, preapproved, or implicitly approved. 
        public PayResponse PayAPIOperations(PayRequest reqPay, bool isPreapproved)
        {
            // Create the PayResponse object
            PayResponse responsePay = new PayResponse();

            try
            {
                // Create the service wrapper object to make the API call
                AdaptivePaymentsService service = new AdaptivePaymentsService();

                // # API call
                // Invoke the Pay method in service wrapper object
                responsePay = service.Pay(reqPay);

                if (responsePay != null)
                {
                    // Response envelope acknowledgement
                    string acknowledgement = "Pay API Operation - ";
                    acknowledgement += responsePay.responseEnvelope.ack.ToString();


                    // # Success values
                    if (responsePay.responseEnvelope.ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
                    {
                        // The pay key, which is a token you use in other Adaptive
                        // Payment APIs (such as the Refund Method) to identify this
                        // payment. The pay key is valid for 3 hours; the payment must
                        // be approved while the pay key is valid.

                        // Once you get success response, user has to redirect to PayPal
                        // for the payment. Construct redirectURL as follows,
                        // `redirectURL=https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_ap-payment&paykey="
                        // + responsePay.payKey;`

                        string url = String.Format("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_ap-payment&paykey={0}", responsePay.payKey);
                        if (isPreapproved)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            //if (response.StatusCode.ToString() == "OK")
                            //{
                            //    Console.WriteLine("PAY MENT SUCCESS");
                            //}
                        }
                        else
                            if (HttpContext.Current != null)
                                HttpContext.Current.Response.Redirect(url);
                        
                    }
                    // # Error Values 
                    else
                    {
                        List<ErrorData> errorMessages = responsePay.error;
                        foreach (ErrorData error in errorMessages)
                        {
                        }
                    }
                }
            }
            // # Exception log    
            catch (System.Exception ex)
            {
                string temp = ex.StackTrace.ToString();  
            }
            return responsePay;
        }

        // # Preapproval API Operation 
        public PreapprovalResponse PreapprovalAPIOperation(decimal rate, string userId, DateTime subscriptionDate)
        {
            // Create the PreapprovalResponse object
            PreapprovalResponse responsePreapproval = new PreapprovalResponse();

            try
            {
                // # PreapprovalRequest
                // The code for the language in which errors are returned
                RequestEnvelope envelopeRequest = new RequestEnvelope();
                envelopeRequest.errorLanguage = "en_US";

                PreapprovalRequest requestPreapproval = new PreapprovalRequest();
                requestPreapproval.requestEnvelope = envelopeRequest;
                requestPreapproval.returnUrl = System.Configuration.ConfigurationManager.AppSettings["RETURN_URL"] + "/DoctorInformation/PayPalSubscription/?pstatus=success&userId=" + userId;
                requestPreapproval.cancelUrl = System.Configuration.ConfigurationManager.AppSettings["CANCEL_URL"] + "/DoctorInformation/PayPalSubscription/?pstatus=cancel&userid=" + userId;
                requestPreapproval.currencyCode = "USD";
                requestPreapproval.startingDate = subscriptionDate.ToString("yyyy-MM-dd");
                requestPreapproval.endingDate = subscriptionDate.AddYears(1).ToString("yyyy-MM-dd");
                requestPreapproval.feesPayer = "EACHRECEIVER";
                requestPreapproval.maxAmountPerPayment = rate;
                requestPreapproval.maxNumberOfPaymentsPerPeriod = 1;
                requestPreapproval.maxTotalAmountOfAllPayments = (12 * rate);
                requestPreapproval.maxNumberOfPayments = 12;
                //requestPreapproval.dateOfMonth = 27;
                requestPreapproval.pinType = "NOT_REQUIRED";
                //if(!string.IsNullOrEmpty(paypalId))
                //    requestPreapproval.senderEmail = paypalId;
                requestPreapproval.paymentPeriod = "DAILY";
                // IPN URL
                //  
                // * PayPal Instant Payment Notification is a call back system that is initiated when a transaction is completed        
                // * The transaction related IPN variables will be received on the call back URL specified in the request       
                // * The IPN variables have to be sent back to the PayPal system for validation, upon validation PayPal will send a response string "VERIFIED" or "INVALID"     
                // * PayPal would continuously resend IPN if a wrong IPN is sent        
                requestPreapproval.ipnNotificationUrl = System.Configuration.ConfigurationManager.AppSettings["IPN_URL"] + "/DoctorInformation/PayPalIPN/";

                // Create the service wrapper object to make the API call
                AdaptivePaymentsService service = new AdaptivePaymentsService();

                // # API call
                // Invoke the Preapproval method in service wrapper object
                responsePreapproval = service.Preapproval(requestPreapproval);

                if (responsePreapproval != null)
                {
                    //// Response envelope acknowledgement
                    //string acknowledgement = "Preapproval API Operation - ";
                    //acknowledgement += responsePreapproval.responseEnvelope.ack.ToString();
                    //logger.Info(acknowledgement + "\n");
                    //Console.WriteLine(acknowledgement + "\n");

                    // # Success values
                    if (responsePreapproval.responseEnvelope.ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
                    {
                        //logger.Info("Preapproval Key : " + responsePreapproval.preapprovalKey + "\n");
                        //Console.WriteLine("Preapproval Key : " + responsePreapproval.preapprovalKey + "\n");

                        // Once you get success response, user has to redirect to PayPal
                        // to preapprove the payment. Construct redirectURL as follows,
                        // `redirectURL=https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_ap-preapproval&preapprovalkey="
                        // + responsePreapproval.preapprovalKey;`          
          
                        string url = String.Format("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_ap-preapproval&preapprovalkey={0}", responsePreapproval.preapprovalKey);
                        if (HttpContext.Current != null)
                            HttpContext.Current.Response.Redirect(url);
                    }
                    // # Error Values
                    else
                    {
                        List<ErrorData> errorMessages = responsePreapproval.error;
                        foreach (ErrorData error in errorMessages)
                        {
                            //logger.Debug("API Error Message : " + error.message);
                            //Console.WriteLine("API Error Message : " + error.message + "\n");
                        }
                    }
                }
            }
            // # Exception log    
            catch (System.Exception ex)
            {
                // Log the exception message       
            }
            return responsePreapproval;
        }

        public PayRequest PreapprovalPayment(string key,string payPalId,string userId,decimal rate)
        {
            // # PayRequest
            // The code for the language in which errors are returned
            RequestEnvelope envelopeRequest = new RequestEnvelope();
            envelopeRequest.errorLanguage = "en_US";

            List<Receiver> listReceiver = new List<Receiver>();

            // Amount to be credited to the receiver's account
            Receiver receive = new Receiver(rate);

            // A receiver's email address
            //receive.email = "abc@paypal.com";
            receive.email = "sanjeev-facilitator@alistechnology.com";
            listReceiver.Add(receive);
            ReceiverList listOfReceivers = new ReceiverList(listReceiver);

            PayRequest requestPay = new PayRequest();
            requestPay.senderEmail = payPalId;
            requestPay.preapprovalKey = key;
            requestPay.receiverList = listOfReceivers;
            requestPay.requestEnvelope = envelopeRequest;
            requestPay.returnUrl = System.Configuration.ConfigurationManager.AppSettings["RETURN_URL"] + "/DoctorInformation/PayPalSubscription/?pstatus=success&userId=" + userId;
            requestPay.cancelUrl = System.Configuration.ConfigurationManager.AppSettings["CANCEL_URL"] + "/DoctorInformation/PayPalSubscription/?pstatus=cancel&userid=" + userId;
            requestPay.currencyCode = "USD";
            requestPay.actionType = "PAY";
            requestPay.ipnNotificationUrl = System.Configuration.ConfigurationManager.AppSettings["IPN_URL"] + "/DoctorInformation/PayPalIPN/";
            return requestPay;
        }

    }
}
