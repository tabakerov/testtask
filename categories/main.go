package main

import (
	"flag"

	"github.com/gin-gonic/gin"

	swaggerfiles "github.com/swaggo/files"
	ginSwagger "github.com/swaggo/gin-swagger"
	"github.com/tabakerov/testtask/categories/controllers"
	docs "github.com/tabakerov/testtask/categories/docs"
	"github.com/tabakerov/testtask/categories/storage"
)

func main() {
	port := flag.String("port", "3000", "Port to run server on")

	router := gin.Default()

	docs.SwaggerInfo.BasePath = "/"
	router.GET("/swagger/*any", ginSwagger.WrapHandler(swaggerfiles.Handler))

	controller := controllers.NewCategoryController(storage.NewCategoryStorage())

	router.GET("/categories", controller.ListCategories)
	router.GET("/categories/:id", controller.GetCategory)
	router.POST("/categories", controller.CreateCategory)
	router.PUT("/categories/:id", controller.UpdateCategory)
	router.DELETE("/categories/:id", controller.DeleteCategory)

	router.Run(":" + *port)
}
