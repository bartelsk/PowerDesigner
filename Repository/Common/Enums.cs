// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace PDRepository.Common
{
    /// <summary>
    /// Permission types of a repository object.
    /// </summary>
    public enum PermissionTypeEnum { NotSet = -1, Listable = 0, Read = 10, Submit = 20, Write = 30, Full = 40 }
}
