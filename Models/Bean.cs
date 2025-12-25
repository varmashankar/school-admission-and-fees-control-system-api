using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    #region MyRegion

    public class Bean
    {
    }

    #endregion

    #region ControllerActions
    public class ControllerActions
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
    }

    #endregion

    #region CheckUserAccess

    public class CheckUserAccess
    {
        public int userId { get; set; }
        public int roleTypeId { get; set; }
        public string controllerName { get; set; }
        public string actionName { get; set; }
    }

    #endregion

    #region TokenResponse
    public class TokenResponse
    {
        public string userId { get; set; }
        public string loginAs { get; set; }
        public string msg { get; set; }

    }

    #endregion

    #region SPResponse

    public class SPResponse
    {
        public int? id { get; set; }
        public string message { get; set; }
        public string executionStatus { get; set; }
    }

    #endregion

    #region Login
    public class Login
    {
        public string creationTimestamp { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    #endregion

    #region LoginResponse

    public class LoginResponse
    {
        public int? id { get; set; }
        public int? roleTypeId { get; set; }
        public string message { get; set; }
        public string executionStatus { get; set; }
    }

    #endregion

    #region ChangePassword
    public class ChangePassword
    {
        public int userId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }

    #endregion

    #region ForgetPassword
    public class ForgetPassword
    {
        public string creationTimestamp { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string otp { get; set; }
        public string password { get; set; }
    }

    #endregion

    #region OTP
    public class OTP
    {
        public string creationTimestamp { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string otp { get; set; }

    }

    #endregion

    #region ActionNames
    public class ActionNames
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public string controllerName { get; set; }
        public string actionName { get; set; }
        public bool? openAccess { get; set; }
        public int roleTypeId { get; set; }
        public bool deleted { get; set; }
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool status { get; set; } = true;
        public List<Permission> permissions { get; set; }
    }

    #endregion

    #region ActionNamesRow
    public class ActionNamesRow
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public string controllerName { get; set; }
        public string actionName { get; set; }
        public bool? openAccess { get; set; }
        public int roleTypeId { get; set; }
        public int? roleId { get; set; }
        public string roleType { get; set; }
        public int? permissionId { get; set; }
        public bool? permission { get; set; }
        public bool deleted { get; set; }
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool status { get; set; } = true;
    }

    #endregion

    #region ActionNamesFilter

    public class ActionNamesFilter
    {
        public int? id { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public string controllerName { get; set; }
        public string actionName { get; set; }
        public bool? openAccess { get; set; }
        public bool? deleted { get; set; } = false;
        public bool? status { get; set; }
        public int? pageNo { get; set; }
    }

    #endregion

    #region Permission
    public class Permission
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? createdById { get; set; }
        public int? controllerNameId { get; set; }
        public string controllerName { get; set; }
        public int? actionNameId { get; set; }
        public string actionName { get; set; }
        public int? roleId { get; set; }
        public string role { get; set; }
        public bool? permission { get; set; }
        public bool deleted { get; set; }
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool status { get; set; } = true;
        public List<Permission> permissionsList { get; set; }
    }

    #endregion

    #region UpdatePermission
    public class UpdatePermission
    {
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? roleId { get; set; }
        public List<ActionNames> actionNamesList { get; set; }
    }

    #endregion

    #region PermissionFilter

    public class PermissionFilter
    {
        public int? id { get; set; }
        public int? createdById { get; set; }
        public int? actionNameId { get; set; }
        public bool? deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; }
        public bool? permission { get; set; }
        public int? userId { get; set; }
        public int? roleId { get; set; }
        public int? roleTypeId { get; set; }
        public int? pageNo { get; set; }
    }

    #endregion

    #region RoleTypes
    public class RoleTypes
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? createdById { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool status { get; set; } = true;
    }

    #endregion

    #region RoleTypesFilter

    public class RoleTypesFilter
    {
        public int? id { get; set; }
        public int? createdById { get; set; }
        public bool deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? pageNo { get; set; }
    }

    #endregion

    #region SuperAdmin
    public class SuperAdmin
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? userId { get; set; }
        public int? createdById { get; set; }
        public int? roleId { get; set; }
        public int? roleTypeId { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool status { get; set; } = true;
    }

    #endregion

    #region GetSuperAdmin
    public class GetSuperAdmin
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }
        public int? roleId { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
    }

    #endregion

    #region SuperAdminFilter

    public class SuperAdminFilter
    {
        public int? id { get; set; }
        public int? createdById { get; set; }
        public int? roleId { get; set; }
        public bool? deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? pageNo { get; set; }
    }

    #endregion

    #region AdminUsers
    public class AdminUsers
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }
        public int roleId { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
        public int? userId { get; set; }
        public int roleTypeId { get; set; }
    }

    #endregion

    #region GetAdminUsers
    public class GetAdminUsers
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }
        public int? roleId { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
    }

    #endregion

    #region AdminUsersFilter

    public class AdminUsersFilter
    {
        public int? id { get; set; }
        public int? createdById { get; set; }
        public int? roleId { get; set; }
        public bool? deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? pageNo { get; set; }
    }

    #endregion

    #region Country
    public class Country
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public string acronym { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
        public List<State> states { get; set; }
        public List<City> cities { get; set; }

    }

    #endregion

    #region GetCountry
    public class GetCountry
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? roleTypeId { get; set; }
        public string acronym { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
        public List<State> states { get; set; }
        public List<City> cities { get; set; }

    }

    #endregion

    #region CountryFilter
    public class CountryFilter
    {
        public int? id { get; set; }
        public int? createdById { get; set; }
        public bool? deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? pageNo { get; set; }
    }

    #endregion

    #region State

    public class State
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? countryId { get; set; }
        public string acronym { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
        public List<City> cities { get; set; }
    }

    #endregion

    #region GetState

    public class GetState
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? roleTypeId { get; set; }
        public int? countryId { get; set; }
        public string country { get; set; }
        public string acronym { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
        public List<City> cities { get; set; }
    }

    #endregion

    #region StateFilter
    public class StateFilter
    {
        public int? id { get; set; }
        public int? createdById { get; set; }
        public int? countryId { get; set; }
        public bool? deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? pageNo { get; set; }

    }

    #endregion

    #region City
    public class City
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? countryId { get; set; }
        public int? stateId { get; set; }
        public string acronym { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
        public List<City> cities { get; set; }
    }

    #endregion

    #region GetCity
    public class GetCity
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? countryId { get; set; }
        public string country { get; set; }
        public int? stateId { get; set; }
        public string state { get; set; }
        public string acronym { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageFile { get; set; }
        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        [DefaultValue(null)]
        public int? deletedById { get; set; }
        [DefaultValue(null)]
        public bool? status { get; set; } = true;
        public List<City> cities { get; set; }
    }

    #endregion

    #region CityFilter
    public class CityFilter
    {
        public int? id { get; set; }
        public int? createdById { get; set; }
        public int? countryId { get; set; }
        public int? stateId { get; set; }
        public bool? deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
        public int? pageNo { get; set; }
    }

    #endregion
}