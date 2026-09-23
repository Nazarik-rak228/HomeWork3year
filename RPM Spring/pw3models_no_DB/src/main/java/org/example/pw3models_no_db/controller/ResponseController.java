package org.example.pw3models_no_db.controller;

import org.example.pw3models_no_db.model.ResponseModel;
import org.example.pw3models_no_db.model.TaskModel;
import org.example.pw3models_no_db.model.UserModel;
import org.example.pw3models_no_db.service.ResponseService;
import org.example.pw3models_no_db.service.TaskService;
import org.example.pw3models_no_db.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;

@Controller
public class ResponseController {
    @Autowired
    private ResponseService responseService;
    @Autowired
    private UserService userService;
    @Autowired
    private TaskService taskService;

    @GetMapping("/responses/list")
    public String allResponses(Model model,
                               @RequestParam(defaultValue = "1") int page,
                               @RequestParam(required = false) String status,
                               @RequestParam(required = false) Integer id) {

        List<ResponseModel> allResponses = responseService.findAll();
        List<UserModel> users = userService.findAll();
        List<TaskModel> tasks = taskService.findAll();

        if (id != null) {
            ResponseModel found = responseService.findById(id);
            allResponses = (found != null) ? List.of(found) : List.of();
        } else if (status != null && !status.trim().isEmpty()) {
            allResponses = responseService.findByStatus(status.trim());
        }

        int pageSize = 10;
        int totalItems = allResponses.size();
        int totalPages = (int) Math.ceil((double) totalItems / pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        int from = (page - 1) * pageSize;
        int to = Math.min(from + pageSize, totalItems);

        model.addAttribute("responses", allResponses.subList(from, to));
        model.addAttribute("currentPage", page);
        model.addAttribute("totalPages", totalPages);
        model.addAttribute("searchStatus", status);
        model.addAttribute("searchId", id);
        model.addAttribute("users", users);
        model.addAttribute("tasks", tasks);
        return "responseList";
    }

    @PostMapping("/responses/add")
    public String addResponse(@RequestParam int userId,
                              @RequestParam int taskId,
                              @RequestParam String message,
                              @RequestParam String status) {

        UserModel user = userService.findById(userId);
        TaskModel task = taskService.findById(taskId);

        if (user == null || task == null) return "redirect:/responses/list";

        ResponseModel response = new ResponseModel(0, user, task, message, status);
        responseService.addResponse(response);
        return "redirect:/responses/list";
    }

    @PostMapping("/responses/update")
    public String updateResponse(@RequestParam int id,
                                 @RequestParam int userId,
                                 @RequestParam int taskId,
                                 @RequestParam String message,
                                 @RequestParam String status) {

        UserModel user = userService.findById(userId);
        TaskModel task = taskService.findById(taskId);
        if (user == null || task == null) return "redirect:/responses/list";

        ResponseModel response = new ResponseModel(id, user, task, message, status);
        responseService.updateResponse(response);
        return "redirect:/responses/list";
    }

    @PostMapping("/responses/delete")
    public String deleteResponse(@RequestParam int id) {
        responseService.deleteResponse(id);
        return "redirect:/responses/list";
    }
}