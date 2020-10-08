using System;
using System.Collections.Generic;
using System.Text;

namespace Groundforce.Common.Utilities.Util
{
    public enum CodeStatus
    {
        Ok = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        Notfound = 404,
        InternalServerError = 500,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504
    }
}
