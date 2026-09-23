package org.example.pw3models_no_db.controller;

import org.example.pw3models_no_db.model.CategoryModel;
import org.example.pw3models_no_db.model.TaskModel;
import org.example.pw3models_no_db.model.UserModel;
import org.example.pw3models_no_db.service.CategoryService;
import org.example.pw3models_no_db.service.TaskService;
import org.example.pw3models_no_db.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

@Controller
public class TaskController {
    @Autowired
    private TaskService taskService;
    @Autowired
    private UserService userService;
    @Autowired
    private CategoryService categoryService;

    @GetMapping("/tasks/list")
    public String allTasks(Model model,
                           @RequestParam(defaultValue = "1") int page,
                           @RequestParam(required = false) String title,
                           @RequestParam(required = false) Integer id) {

        List<TaskModel> allTasks = taskService.findAll();

        if (id != null) {
            TaskModel found = taskService.findById(id);
            allTasks = (found != null) ? List.of(found) : List.of();
        } else if (title != null && !title.trim().isEmpty()) {
            allTasks = taskService.findByTitle(title.trim());
        }

        int pageSize = 10;
        int totalItems = allTasks.size();
        int totalPages = (int) Math.ceil((double) totalItems / pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        int from = (page - 1) * pageSize;
        int to = Math.min(from + pageSize, totalItems);

        model.addAttribute("tasks", allTasks.subList(from, to));
        model.addAttribute("currentPage", page);
        model.addAttribute("totalPages", totalPages);
        model.addAttribute("searchTitle", title);
        model.addAttribute("searchId", id);
        model.addAttribute("users", userService.findAll());
        model.addAttribute("categories", categoryService.findAll());

        return "taskList";
    }
    @PostMapping("/tasks/add")
    public String addTask(@RequestParam String title,
                          @RequestParam String description,
                          @RequestParam String deadline,
                          @RequestParam double price,
                          @RequestParam int userId,
                          @RequestParam int categoryId) {
        try {
            LocalDate date = LocalDate.parse(deadline);

            // Находим User и Category
            UserModel user = userService.findById(userId);
            CategoryModel category = categoryService.findById(categoryId);

            if (user == null || category == null) {
                System.out.println("Ошибка: пользователь или категория не найдены!");
                return "redirect:/tasks/list?error=notFound";
            }

            // Создаем задачу
            TaskModel task = new TaskModel(0, title, description, date, price, user, category);
            taskService.addTask(task);

            System.out.println("Задача добавлена успешно: " + task);

            return "redirect:/tasks/list";  // Перенаправление после успешного добавления
        } catch (Exception e) {
            System.out.println("Ошибка при добавлении задачи: " + e.getMessage());
            return "redirect:/tasks/list?error=invalidData";  // В случае ошибки
        }
    }

    @PostMapping("/tasks/update")
    public String updateTask(@RequestParam int id,
                             @RequestParam String title,
                             @RequestParam String description,
                             @RequestParam String deadline,
                             @RequestParam double price,
                             @RequestParam int userId,
                             @RequestParam int categoryId) {
        try {
            LocalDate date = LocalDate.parse(deadline);
            UserModel user = userService.findById(userId);
            CategoryModel category = categoryService.findById(categoryId);

            if (user == null || category == null) return "redirect:/tasks/list";

            TaskModel task = new TaskModel(id, title, description, date, price, user, category);
            taskService.updateTask(task);
            return "redirect:/tasks/list";
        } catch (Exception e) {
            return "redirect:/tasks/list?error=invalidDate";
        }
    }

    @PostMapping("/tasks/delete")
    public String deleteTask(@RequestParam int id) {
        taskService.deleteTask(id);
        return "redirect:/tasks/list";
    }
}