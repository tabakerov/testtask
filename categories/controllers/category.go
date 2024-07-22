package controllers

import (
	"net/http"
	"strconv"

	"github.com/gin-gonic/gin"
	"github.com/go-playground/validator/v10"
	"github.com/sirupsen/logrus"

	"github.com/tabakerov/testtask/categories/models"
	"github.com/tabakerov/testtask/categories/storage"
)

type CategoryController struct {
	storage  storage.CategoryStorage
	validate *validator.Validate
	log      *logrus.Logger
}

func NewCategoryController(service storage.CategoryStorage) *CategoryController {
	return &CategoryController{
		storage:  service,
		validate: validator.New(),
		log:      logrus.New(),
	}
}

// @Summary List all categories
// @Description get categories
// @ID get-categories
// @Produce json
// @Success 200 {array} models.Category
// @Router /categories [get]
func (c *CategoryController) ListCategories(ctx *gin.Context) {
	c.log.Info("Fetching all categories")
	ctx.JSON(http.StatusOK, c.storage.GetAllCategories())
}

// @Summary Get a category
// @Description get category by ID
// @ID get-category
// @Produce json
// @Param id path string true "Category ID"
// @Success 200 {object} models.Category
// @Failure 404 {object} object{error=string}
// @Router /categories/{id} [get]
func (c *CategoryController) GetCategory(ctx *gin.Context) {
	id := ctx.Param("id")
	intID, err := strconv.Atoi(id)
	if err != nil {
		c.log.WithError(err).Warn("Bad url parameter")
		ctx.JSON(http.StatusBadRequest, gin.H{"error": "invalid category ID"})
		return
	}

	c.log.WithFields(logrus.Fields{"id": intID}).Info("Fetching category")
	if category, found := c.storage.GetCategory(intID); found {
		ctx.JSON(http.StatusOK, category)
	} else {
		c.log.WithFields(logrus.Fields{"id": id}).Warn("Category not found")
		ctx.JSON(http.StatusNotFound, gin.H{"error": "category not found"})
	}
}

// @Summary Create a category
// @Description add a new category
// @ID create-category
// @Accept json
// @Produce json
// @Param category body models.CreateCategoryRequest true "Category object"
// @Success 201 {object} models.Category
// @Router /categories [post]
func (c *CategoryController) CreateCategory(ctx *gin.Context) {
	var category models.CreateCategoryRequest
	if err := ctx.ShouldBindJSON(&category); err != nil {
		c.log.WithError(err).Warn("Bad request")
		ctx.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	validatonErr := c.validate.Struct(category)
	if validatonErr != nil {
		c.log.WithError(validatonErr).Warn("Validation error")
		ctx.JSON(http.StatusBadRequest, gin.H{"error": validatonErr.Error()})
		return
	}

	createdCategory := c.storage.CreateCategory(category)
	c.log.WithFields(logrus.Fields{"id": createdCategory.ID}).Info("Category created")
	ctx.JSON(http.StatusCreated, createdCategory)
}

// @Summary Update a category
// @Description update category by ID
// @ID update-category
// @Accept json
// @Produce json
// @Param id path string true "Category ID"
// @Param category body models.Category true "Category object"
// @Success 200 {object} models.Category
// @Router /categories/{id} [put]
func (c *CategoryController) UpdateCategory(ctx *gin.Context) {
	id := ctx.Param("id")
	intID, err := strconv.Atoi(id)
	if err != nil {
		c.log.WithError(err).Warn("Bad url parameter")
		ctx.JSON(http.StatusBadRequest, gin.H{"error": "invalid category ID"})
		return
	}

	var category models.Category
	if err := ctx.ShouldBindJSON(&category); err != nil {
		c.log.WithError(err).Warn("Bad request")
		ctx.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	validatonErr := c.validate.Struct(category)
	if validatonErr != nil {
		c.log.WithError(validatonErr).Warn("Validation error")
		ctx.JSON(http.StatusBadRequest, gin.H{"error": validatonErr.Error()})
		return
	}

	c.storage.UpdateCategory(intID, category)
	c.log.WithFields(logrus.Fields{"id": category.ID}).Info("Category updated")
	ctx.JSON(http.StatusOK, category)
}

// @Summary Delete a category
// @Description delete category by ID
// @ID delete-category
// @Param id path string true "Category ID"
// @Success 200 {object} object{status=string}
// @Router /categories/{id} [delete]
func (c *CategoryController) DeleteCategory(ctx *gin.Context) {
	id := ctx.Param("id")
	intID, err := strconv.Atoi(id)
	if err != nil {
		c.log.WithError(err).Warn("Bad url parameter")

		ctx.JSON(http.StatusBadRequest, gin.H{"error": "invalid category ID"})
		return
	}

	c.storage.DeleteCategory(intID)
	c.log.WithFields(logrus.Fields{"id": intID}).Info("Category deleted")
	ctx.JSON(http.StatusOK, gin.H{"status": "deleted"})
}
