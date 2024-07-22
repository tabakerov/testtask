package models

type Category struct {
	ID   int    `json:"id"`
	Name string `json:"name"`
}

type CreateCategoryRequest struct {
	Name string `json:"name" validate:"required,min=3"`
}
