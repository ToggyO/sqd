﻿using System.Collections.Generic;
using Squadio.DTO.Companies;

namespace Squadio.DTO.Users
{
    public class UserWithCompaniesDTO
    {
        public UserDTO User { get; set; }
        public IEnumerable<CompanyUserDTO> Companies { get; set; }
    }
}