package routers

import (
	"downloadServer/controllers"
	"github.com/astaxie/beego"
)

func init() {

	beego.Router("/", &controllers.MainController{})
	beego.Router("/down", &controllers.MainController{}, "get,post:DownLoad")
}
