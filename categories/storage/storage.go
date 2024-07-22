package storage

import (
	"sync"

	"github.com/tabakerov/testtask/categories/models"
)

func NewCategoryStorage() *categoryStorage {
	return &categoryStorage{
		categories: make(map[int]models.Category),
		rwMutex:    sync.RWMutex{},
		dbCounter:  0,
	}
}

type categoryStorage struct {
	categories map[int]models.Category
	rwMutex    sync.RWMutex
	dbCounter  int
}

type CategoryStorage interface {
	GetAllCategories() []models.Category
	GetCategory(id int) (models.Category, bool)
	CreateCategory(c models.CreateCategoryRequest) models.Category
	UpdateCategory(id int, c models.Category) bool
	DeleteCategory(id int) bool
}

func (s *categoryStorage) GetAllCategories() []models.Category {
	s.rwMutex.RLock()
	defer s.rwMutex.RUnlock()

	result := make([]models.Category, 0)
	for _, v := range s.categories {
		result = append(result, v)
	}
	return result
}

func (s *categoryStorage) GetCategory(id int) (models.Category, bool) {
	s.rwMutex.RLock()
	defer s.rwMutex.RUnlock()

	c, ok := s.categories[id]
	return c, ok
}

func (s *categoryStorage) CreateCategory(c models.CreateCategoryRequest) models.Category {
	s.rwMutex.Lock()
	defer s.rwMutex.Unlock()

	s.categories[s.dbCounter] = models.Category{
		ID:   s.dbCounter,
		Name: c.Name,
	}
	s.dbCounter++
	return s.categories[s.dbCounter-1]
}

func (s *categoryStorage) UpdateCategory(id int, c models.Category) bool {
	s.rwMutex.Lock()
	defer s.rwMutex.Unlock()

	if _, ok := s.categories[id]; !ok {
		return false
	}
	s.categories[id] = c
	return true
}

func (s *categoryStorage) DeleteCategory(id int) bool {
	s.rwMutex.Lock()
	defer s.rwMutex.Unlock()

	if _, ok := s.categories[id]; !ok {
		return false
	}
	delete(s.categories, id)
	return true
}
