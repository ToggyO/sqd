﻿using System.Threading.Tasks;
 using Squadio.Common.Models.Responses;
 using Squadio.DTO.Models.SignUp;

 namespace Squadio.BLL.Services.SignUp
{
    public interface ISignUpService
    {
        Task<Response> SimpleSignUp(SignUpSimpleDTO dto);
    }
}