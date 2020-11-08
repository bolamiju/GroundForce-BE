using System;

namespace Groundforce.Common.Utilities.Helpers
{
    public static class AccountNumberValidator
    {
        public static bool ValidateNUBANAccount(string bankName, string accountNumber)
        {
            bankName = bankName.Replace(" ", "").Replace("(", "").Replace(")", "").ToLower();

            int code = NUBANCodeForBank(bankName);

            string bankCode;
            if (code.ToString().Length == 2)
                bankCode = "0" + code.ToString();
            else
                bankCode = code.ToString();

            bool result = false;
            try
            {
                if (bankCode.Trim().Length == 3 && accountNumber.Trim().Length == 10)
                {
                    var nuban = bankCode + accountNumber.Remove(9, 1);
                    var checkDigit = Convert.ToInt32(accountNumber.Substring(9, 1));
                    var calCheckDigit = CheckNUBANDigit(nuban);
                    calCheckDigit = calCheckDigit != 10 ? calCheckDigit : 0;
                    result = (checkDigit == calCheckDigit);
                }
            }
            catch { }
            return result;
        }

        private static int NUBANCodeForBank(string bankName)
        {
            int code = 0;
            if (bankName == BankCodes.accessbankplc.ToString())
                code = BankCodes.accessbankplc.GetHashCode();
            else if (bankName == BankCodes.accessbankplcdiamond.ToString())
                code = BankCodes.accessbankplcdiamond.GetHashCode();
            else if (bankName == BankCodes.ecobanknigeria.ToString())
                code = BankCodes.ecobanknigeria.GetHashCode();
            else if (bankName == BankCodes.enterprisebankplc.ToString())
                code = BankCodes.enterprisebankplc.GetHashCode();
            else if (bankName == BankCodes.fidelitybankplc.ToString())
                code = BankCodes.fidelitybankplc.GetHashCode();
            else if (bankName == BankCodes.firstbankofnigeriaplc.ToString())
                code = BankCodes.firstbankofnigeriaplc.GetHashCode();
            else if (bankName == BankCodes.firstcitymonumentbank.ToString())
                code = BankCodes.firstcitymonumentbank.GetHashCode();
            else if (bankName == BankCodes.guarantytrustbankplc.ToString())
                code = BankCodes.guarantytrustbankplc.GetHashCode();
            else if (bankName == BankCodes.heritagebank.ToString())
                code = BankCodes.heritagebank.GetHashCode();
            else if (bankName == BankCodes.jaizbank.ToString())
                code = BankCodes.jaizbank.GetHashCode();
            else if (bankName == BankCodes.keystonebankltd.ToString())
                code = BankCodes.keystonebankltd.GetHashCode();
            else if (bankName == BankCodes.mainstreetbankplc.ToString())
                code = BankCodes.mainstreetbankplc.GetHashCode();
            else if (bankName == BankCodes.polarisbank.ToString())
                code = BankCodes.polarisbank.GetHashCode();
            else if (bankName == BankCodes.stanbicibtcplc.ToString())
                code = BankCodes.stanbicibtcplc.GetHashCode();
            else if (bankName == BankCodes.sterlingbankplc.ToString())
                code = BankCodes.sterlingbankplc.GetHashCode();
            else if (bankName == BankCodes.unionbanknigeriaplc.ToString())
                code = BankCodes.unionbanknigeriaplc.GetHashCode();
            else if (bankName == BankCodes.unitedbankforafricaplc.ToString())
                code = BankCodes.unitedbankforafricaplc.GetHashCode();
            else if (bankName == BankCodes.unitybankplc.ToString())
                code = BankCodes.unitybankplc.GetHashCode();
            else if (bankName == BankCodes.wemabankplc.ToString())
                code = BankCodes.wemabankplc.GetHashCode();
            else if (bankName == BankCodes.zenithbankinternational.ToString())
                code = BankCodes.zenithbankinternational.GetHashCode();

            return code;
        }

        private static int CheckNUBANDigit(string nuban)
        {
            var nubanArray = new int[nuban.Length];
            for (int i = 0; i < nuban.Length; i++)
            {
                nubanArray[i] = Convert.ToInt32(nuban[i].ToString());
            }
            var nubanSum = (nubanArray[0] * 3) + (nubanArray[1] * 7) + (nubanArray[2] * 3) + (nubanArray[3] * 3) +
                           (nubanArray[4] * 7) + (nubanArray[5] * 3) + (nubanArray[6] * 3) + (nubanArray[7] * 7) +
                           (nubanArray[8] * 3) + (nubanArray[9] * 3) + (nubanArray[10] * 7) + (nubanArray[11] * 3);
            return 10 - (nubanSum % 10);
        }
    }
}
