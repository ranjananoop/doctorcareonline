using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.InteropServices;

namespace DOCVIDEO.PayPalWrapper
{
    

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct BasicInformation
    {
        private PayPalIPN _IPN;
        public BasicInformation(ref PayPalIPN PayPalIPN)
        {
            this = new BasicInformation();
            this.IPN = PayPalIPN;
        }

        public PayPalIPN IPN
        {
            get
            {
                return this._IPN;
            }
            set
            {
                this._IPN = value;
            }
        }
        public decimal mc_gross
        {
            get
            {
                return Convert.ToDecimal(this.IPN.AllInformation["mc_gross"]);
            }
        }
        public Address_Status_Variables address_status
        {
            get
            {
                Type enumType = typeof(Address_Status_Variables);
                return (Address_Status_Variables)Conversions.ToByte(Enum.Parse(enumType, this.IPN.AllInformation["payment_status"]));
            }
        }
        public string payer_id
        {
            get
            {
                return this.IPN.AllInformation["payer_id"];
            }
        }
        public decimal tax
        {
            get
            {
                return Convert.ToDecimal(this.IPN.AllInformation["tax"]);
            }
        }
        public string address_street
        {
            get
            {
                return this.IPN.AllInformation["address_street"];
            }
        }
        public DateTime payment_date
        {
            get
            {
                return Convert.ToDateTime(this.IPN.AllInformation["payment_date"]);
            }
        }
        public Payment_Status_Variables payment_status
        {
            get
            {
                Type enumType = typeof(Payment_Status_Variables);
                return (Payment_Status_Variables)Conversions.ToByte(Enum.Parse(enumType, this.IPN.AllInformation["payment_status"]));
            }
        }
        public string charset
        {
            get
            {
                return this.IPN.AllInformation["charset"];
            }
        }
        public string address_zip
        {
            get
            {
                return this.IPN.AllInformation["address_zip"];
            }
        }
        public string first_name
        {
            get
            {
                return this.IPN.AllInformation["first_name"];
            }
        }
        public string address_country_code
        {
            get
            {
                return this.IPN.AllInformation["address_country_code"];
            }
        }
        public string address_name
        {
            get
            {
                return this.IPN.AllInformation["address_name"];
            }
        }
        public string notify_version
        {
            get
            {
                return this.IPN.AllInformation["notify_version"];
            }
        }
        public string custom
        {
            get
            {
                return this.IPN.AllInformation["custom"];
            }
        }
        public Payer_Status_Variables payer_status
        {
            get
            {
                Type enumType = typeof(Payer_Status_Variables);
                return (Payer_Status_Variables)Conversions.ToByte(Enum.Parse(enumType, this.IPN.AllInformation["payment_status"]));
            }
        }
        public string business
        {
            get
            {
                return this.IPN.AllInformation["business"];
            }
        }
        public string address_country
        {
            get
            {
                return this.IPN.AllInformation["address_country"];
            }
        }
        public string address_city
        {
            get
            {
                return this.IPN.AllInformation["address_city"];
            }
        }
        public string quantity
        {
            get
            {
                return this.IPN.AllInformation["quantity"];
            }
        }
        public string verify_sign
        {
            get
            {
                return this.IPN.AllInformation["verify_sign"];
            }
        }
        public string payer_email
        {
            get
            {
                return this.IPN.AllInformation["payer_email"];
            }
        }
        public string txn_id
        {
            get
            {
                return this.IPN.AllInformation["txn_id"];
            }
        }
        public payment_type_variables payment_type
        {
            get
            {
                Type enumType = typeof(payment_type_variables);
                return (payment_type_variables)Conversions.ToByte(Enum.Parse(enumType, this.IPN.AllInformation["payment_status"]));
            }
        }
        public string last_name
        {
            get
            {
                return this.IPN.AllInformation["last_name"];
            }
        }
        public string address_state
        {
            get
            {
                return this.IPN.AllInformation["address_state"];
            }
        }
        public string receiver_email
        {
            get
            {
                return this.IPN.AllInformation["receiver_email"];
            }
        }
        public string receiver_id
        {
            get
            {
                return this.IPN.AllInformation["receiver_id"];
            }
        }
        public pending_reason_variables pending_reason
        {
            get
            {
                Type enumType = typeof(pending_reason_variables);
                return (pending_reason_variables)Conversions.ToByte(Enum.Parse(enumType, this.IPN.AllInformation["payment_status"]));
            }
        }
        public txn_type_variables txn_type
        {
            get
            {
                Type enumType = typeof(txn_type_variables);
                return (txn_type_variables)Conversions.ToByte(Enum.Parse(enumType, this.IPN.AllInformation["payment_status"]));
            }
        }
        public string item_name
        {
            get
            {
                return this.IPN.AllInformation["item_name"];
            }
        }
        public mc_currency_variables mc_currency
        {
            get
            {
                Type enumType = typeof(mc_currency_variables);
                return (mc_currency_variables)Conversions.ToByte(Enum.Parse(enumType, this.IPN.AllInformation["mc_currency"]));
            }
        }
        public string item_number
        {
            get
            {
                return this.IPN.AllInformation["item_number"];
            }
        }
        public string residence_country
        {
            get
            {
                return this.IPN.AllInformation["residence_country"];
            }
        }
        public string test_ipn
        {
            get
            {
                return this.IPN.AllInformation["test_ipn"];
            }
        }
        public decimal payment_gross
        {
            get
            {
                return Conversions.ToDecimal(this.IPN.AllInformation["payment_gross"]);
            }
        }
        public decimal shipping
        {
            get
            {
                return Conversions.ToDecimal(this.IPN.AllInformation["shipping"]);
            }
        }
        public enum Address_Status_Variables : byte
        {
            confirmed = 0,
            unconfirmed = 1
        }

        public enum mc_currency_variables : byte
        {
            AUD = 0,
            CAD = 1,
            USD = 15
        }

        public enum Payer_Status_Variables : byte
        {
            unverified = 1,
            verified = 0
        }

        public enum Payment_Status_Variables : byte
        {
            Canceled_Reversal = 0,
            Completed = 1,
            Denied = 2,
            Expired = 3,
            Failed = 4,
            Pending = 5,
            Processed = 6,
            Refunded = 7,
            Reversed = 8,
            Voided = 9
        }

        public enum payment_type_variables : byte
        {
            echeck = 0,
            instant = 1
        }

        public enum pending_reason_variables : byte
        {
            address = 0,
            authorization = 1,
            echeck = 2,
            intl = 3,
            multicurrency = 4,
            other = 8,
            unilateral = 5,
            upgrade = 6,
            verify = 7
        }

        public enum txn_type_variables : byte
        {
            cart = 0,
            express_checkout = 1,
            send_money = 2,
            virtual_terminal = 3,
            web_accept = 4
        }
    }
}
