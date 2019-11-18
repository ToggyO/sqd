using System;

namespace Squadio.Domain.Enums
{
    public enum Role
    {
        User,
        Admin
    }

    public struct RoleGuid
    {
        /// <summary>
        /// bf22a3fd-f7eb-4871-b84a-4d8ff51d0f7d
        /// </summary>
        public static Guid User = Guid.Parse("bf22a3fd-f7eb-4871-b84a-4d8ff51d0f7d");
        /// <summary>
        /// 21085a86-af36-4539-953f-00eb6fffd9d1
        /// </summary>
        public static Guid Admin = Guid.Parse("21085a86-af36-4539-953f-00eb6fffd9d1");
    }
}