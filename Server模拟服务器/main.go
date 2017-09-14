package main

import (
	_ "downloadServer/routers"
	"github.com/astaxie/beego"
)

func main() {
	beego.SetStaticPath("down", "/DownFile") //url string, path string
	beego.Run()
}
