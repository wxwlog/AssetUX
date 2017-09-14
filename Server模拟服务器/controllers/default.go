package controllers

import (
	"github.com/astaxie/beego"
	"github.com/astaxie/beego/logs"
)

type MainController struct {
	beego.Controller
}

func (c *MainController) Get() {
	c.Data["Website"] = "beego.me"
	c.Data["Email"] = "astaxie@gmail.com"
	c.TplName = "index.tpl"
}

func (c *MainController) DownLoad() {
	inputs := c.Input()

	gameName := inputs.Get("gameName")
	fileName := inputs.Get("file")

	logs.Debug("file =", fileName+" in "+gameName)

	c.Ctx.Output.Download("DownFile/" + gameName + "/" + fileName)
	c.Ctx.WriteString("ok")
}
