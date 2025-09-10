﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DVLD_2_my.GlobalClasses
{
    public class clsValidations
    {
        public enum enPersonColSelect
        {
            ePersonID = 1, eNationalNo = 2, eFirstName = 3, eSecondName = 4, eThirdName = 5,
            eLastName = 6, eNationality = 7, eGender = 8, ePhone = 9, eEmail = 10
        }

        static public bool ValidateName(string NameRegex)
        {
            return Regex.IsMatch(NameRegex, "@^[a-zA-Z_-]+$");
        }

        static public bool NationalityOrGender(string NationalityOrGender)
        {
            return Regex.IsMatch(NationalityOrGender, "@^[a-zA-Z]+$");
        }
        
        //static public void ValidateName()
        //{
            
        //    const string EmailRegex = @"^\S+@\S+\.\S+$";
        //    const string NumericRegex = @"^[0-9]+$";
        //    const string NationalNoRegex = @"^[A-Za-z][0-9]+$";
        //}

        static public bool ValidatePhone(string Phone)
        {
            return Regex.IsMatch(Phone, "@^[0-9-+]+$");
        }


    
}
