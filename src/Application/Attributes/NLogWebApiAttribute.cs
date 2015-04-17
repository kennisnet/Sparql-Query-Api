namespace Trezorix.Sparql.Api.Application.Attributes {
  using System.Web.Http.Controllers;
  using System.Web.Http.Filters;

  using NLog;

  /// <summary>
  /// Write WebApi requests to NLog.
  /// </summary>
  public class NLogWebApiAttribute : ActionFilterAttribute {
    /// <summary>
    /// Occurs after the action method is invoked.
    /// </summary>
    /// <param name="actionExecutedContext">
    /// The action executed context.
    /// </param>
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
      var logger = GetLogger(actionExecutedContext.ActionContext.ControllerContext.Controller);
      var request = actionExecutedContext.ActionContext.Request;
      if (actionExecutedContext.Exception != null) {
        logger.Error(string.Format("Exception during execution of {0}", request.RequestUri), actionExecutedContext.Exception);
      }

      var response = actionExecutedContext.Response;
      if (response == null) {
        return;
      }

      if (response.IsSuccessStatusCode) {
        logger.Debug("StatusCode {0} {1} from {2}", (int)response.StatusCode, response.ReasonPhrase, request.RequestUri);
      }
      else {
        logger.Info("StatusCode {0} {1} from {2}", (int)response.StatusCode, response.ReasonPhrase, request.RequestUri);
        foreach (var kvp in actionExecutedContext.ActionContext.ActionArguments) {
          logger.Debug("Argument {0} = {1}", kvp.Key, kvp.Value);
        }
      }
    }

    /// <summary>
    /// Occurs before the action method is invoked.
    /// </summary>
    /// <param name="actionContext">
    /// The action context.
    /// </param>
    public override void OnActionExecuting(HttpActionContext actionContext) {
      var logger = GetLogger(actionContext.ControllerContext.Controller);
      logger.Debug("Executing {0}", actionContext.Request.RequestUri);
      foreach (var kvp in actionContext.ActionArguments) {
        logger.Trace("Argument {0} = {1}", kvp.Key, kvp.Value);
      }
    }

    /// <summary>
    /// Gets logger.
    /// </summary>
    /// <param name="controller">
    /// The controller.
    /// </param>
    /// <returns>
    /// The <see cref="Logger"/>.
    /// </returns>
    private static Logger GetLogger(IHttpController controller) {
      return LogManager.GetLogger(controller.GetType().FullName);
    }
  }
}