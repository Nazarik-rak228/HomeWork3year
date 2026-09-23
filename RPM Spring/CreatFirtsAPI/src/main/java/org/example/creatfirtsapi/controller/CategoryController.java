package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.example.creatfirtsapi.dto.CategoryDto;
import org.example.creatfirtsapi.repository.CategoryRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/category")
@Tag(name = "Categories", description = "CRUD для категорий")
public class CategoryController {

    private final CategoryRepository categoryRepository;

    public CategoryController(CategoryRepository categoryRepository) {
        this.categoryRepository = categoryRepository;
    }

    // GET ALL
    @Operation(summary = "Получить все категории", description = "Возвращает список всех категорий")
    @GetMapping
    public List<CategoryDto> getAll() {
        return categoryRepository.findAll();
    }

    // GET BY ID
    @Operation(summary = "Получить категорию по id", description = "Возвращает категорию по её id")
    @GetMapping("/{id}")
    public ResponseEntity getById(@PathVariable int id) {
        return categoryRepository.findById(id)
                .<ResponseEntity>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND).body("Category not found"));
    }

    // CREATE
    @Operation(summary = "Добавить категорию", description = "Создаёт новую категорию")
    @PostMapping
    public ResponseEntity addCategory(@Valid @RequestBody CategoryDto category) {
        categoryRepository.save(category);
        return ResponseEntity.status(HttpStatus.CREATED).body(category);
    }

    // UPDATE
    @Operation(summary = "Обновить категорию", description = "Обновляет категорию по id")
    @PutMapping
    public ResponseEntity updateCategory(@Valid @RequestBody CategoryDto category) {

        if (!categoryRepository.existsById(category.getId())) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body("Category not found");
        }

        categoryRepository.save(category);
        return ResponseEntity.ok(category);
    }

    // DELETE
    @Operation(summary = "Удалить категорию", description = "Удаляет категорию по id")
    @DeleteMapping("/{id}")
    public ResponseEntity deleteCategory(@PathVariable int id) {

        if (!categoryRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body("Category not found");
        }

        categoryRepository.deleteById(id);
        return ResponseEntity.ok("Deleted");
    }
}