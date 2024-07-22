package controllers_test

import (
	"net/http"
	"net/http/httptest"
	"testing"

	"github.com/gin-gonic/gin"
	"github.com/stretchr/testify/assert"

	"github.com/tabakerov/testtask/categories/controllers"
	"github.com/tabakerov/testtask/categories/storage"
)

func TestGetCategories(t *testing.T) {
	service := storage.NewCategoryStorage()
	controller := controllers.NewCategoryController(service)

	router := gin.Default()
	router.GET("/categories", controller.ListCategories)

	w := httptest.NewRecorder()
	req, _ := http.NewRequest("GET", "/categories", nil)
	router.ServeHTTP(w, req)

	assert.Equal(t, http.StatusOK, w.Code)
	assert.Equal(t, "[]", w.Body.String())
}

func TestCreateCategory(t *testing.T) {
	service := storage.NewCategoryStorage()
	controller := controllers.NewCategoryController(service)

	router := gin.Default()
	router.POST("/categories", controller.CreateCategory)

	w := httptest.NewRecorder()
	req, _ := http.NewRequest("POST", "/categories", nil)
	router.ServeHTTP(w, req)

	assert.Equal(t, http.StatusBadRequest, w.Code)
}
