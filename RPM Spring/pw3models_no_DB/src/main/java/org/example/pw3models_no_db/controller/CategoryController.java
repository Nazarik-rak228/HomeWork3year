package org.example.pw3models_no_db.controller;

import org.example.pw3models_no_db.model.CategoryModel;
import org.example.pw3models_no_db.service.CategoryService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;

@Controller
public class CategoryController {
    @Autowired
    private CategoryService categoryService;

    @GetMapping("/categories/list")
    public String allCategories(Model model,
                                @RequestParam(defaultValue = "1") int page,
                                @RequestParam(required = false) String catName,
                                @RequestParam(required = false) Integer id) {

        List<CategoryModel> allCategories = categoryService.findAll();

        if (id != null) {
            CategoryModel found = categoryService.findById(id);
            allCategories = (found != null) ? List.of(found) : List.of();
        } else if (catName != null && !catName.trim().isEmpty()) {
            allCategories = categoryService.findByCatName(catName.trim());
        }

        int pageSize = 10;
        int totalItems = allCategories.size();
        int totalPages = (int) Math.ceil((double) totalItems / pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        int from = (page - 1) * pageSize;
        int to = Math.min(from + pageSize, totalItems);

        model.addAttribute("categories", allCategories.subList(from, to));
        model.addAttribute("currentPage", page);
        model.addAttribute("totalPages", totalPages);
        model.addAttribute("searchCatName", catName);
        model.addAttribute("searchId", id);

        return "categoryList";
    }

    @PostMapping("/categories/add")
    public String addCategory(@RequestParam String catName) {
        CategoryModel category = new CategoryModel(0, catName);
        categoryService.addCategory(category);
        return "redirect:/categories/list";
    }

    @PostMapping("/categories/update")
    public String updateCategory(@RequestParam int id,
                                 @RequestParam String catName) {
        CategoryModel category = new CategoryModel(id, catName);
        categoryService.updateCategory(category);
        return "redirect:/categories/list";
    }

    @PostMapping("/categories/delete")
    public String deleteCategory(@RequestParam int id) {
        categoryService.deleteCategory(id);
        return "redirect:/categories/list";
    }
}