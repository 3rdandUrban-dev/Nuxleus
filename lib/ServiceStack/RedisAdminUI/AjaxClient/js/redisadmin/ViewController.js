/**
 * Created by IntelliJ IDEA.
 * User: mythz
 * Date: 18-Jul-2010
 * Time: 00:13:45
 * To change this template use File | Settings | File Templates.
 */

goog.provide("redisadmin.ViewController");

/**
 * Base controller
 * @param {string} rootEl name of root element.
 * @param {?Object} the main app.
 * @constructor
 */
redisadmin.ViewController = function(rootEl, app)
{
    this.app = app;
    this.rootEl = rootEl;
    goog.events.EventTarget.call(this);
}
goog.inherits(redisadmin.ViewController, goog.ui.Component);
