package main

import (
	"flag"

	"github.com/gin-gonic/gin"

	swaggerfiles "github.com/swaggo/files"
	ginSwagger "github.com/swaggo/gin-swagger"
	"github.com/tabakerov/testtask/categories/controllers"
	docs "github.com/tabakerov/testtask/categories/docs"
	"github.com/tabakerov/testtask/categories/storage"
	"go.opentelemetry.io/contrib/instrumentation/github.com/gin-gonic/gin/otelgin"
)

func main() {
	port := flag.String("port", "3000", "Port to run server on")

	router := gin.Default()

	InitTracer()

	docs.SwaggerInfo.BasePath = "/"
	router.GET("/swagger/*any", ginSwagger.WrapHandler(swaggerfiles.Handler))

	router.Use(otelgin.Middleware("go-categories-service"))

	controller := controllers.NewCategoryController(storage.NewCategoryStorage())

	router.GET("/categories", controller.ListCategories)
	router.GET("/categories/:id", controller.GetCategory)
	router.POST("/categories", controller.CreateCategory)
	router.PUT("/categories/:id", controller.UpdateCategory)
	router.DELETE("/categories/:id", controller.DeleteCategory)

	router.Run(":" + *port)
}
