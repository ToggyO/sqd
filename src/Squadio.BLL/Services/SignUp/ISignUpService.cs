﻿using System.Threading.Tasks;
 using Squadio.Common.Models.Responses;
 using Squadio.DTO.SignUp;

 namespace Squadio.BLL.Services.SignUp
{
    public interface ISignUpService
    {
        Task<Response> SignUp(SignUpSimpleDTO dto);
    }
}