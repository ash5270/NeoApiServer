using System;
using NeoServer.Models;
using System.Text.RegularExpressions;

namespace NeoServer.Util;

public class DataValidator
{
    public DataValidator()
    {

    }   

    public bool EmailValidator(string email)
    {
        string pattern =@"^[a-zA-Z0-9+-\_.]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        var result = Regex.IsMatch(pattern,email);
        return result;
    }

    public bool IDValidator(string ID)
    {
        //TODO : ID 정규식 추가 
        string pattern = @"*.";
        //var result= Regex.IsMatch(ID,pattern);
        return true;
    }

    public bool PasswordValidator(string password)
    {
        //TODO : password 정규식 추가 
        //string pattern = @"*.";
        //return Regex.IsMatch(password,pattern);
        return true;
    }

    public bool RegisterUserValidator(SignUpData data)
    {
        if(!EmailValidator(data.Email))
            return false;
        if(!IDValidator(data.ID))
            return false;
        if(!PasswordValidator(data.Password))
            return false;
        return true;
    }

    public bool LoginUserValidator(LoginData data)
    {
        if(!IDValidator(data.ID))
            return false;
        if(!PasswordValidator(data.Password))
            return false;
        return true;
    }
}