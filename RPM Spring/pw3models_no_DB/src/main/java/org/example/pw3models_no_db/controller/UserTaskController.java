package org.example.pw3models_no_db.controller;

import org.example.pw3models_no_db.model.TaskModel;
import org.example.pw3models_no_db.model.UserModel;
import org.example.pw3models_no_db.model.UserTaskModel;
import org.example.pw3models_no_db.service.TaskService;
import org.example.pw3models_no_db.service.UserService;
import org.example.pw3models_no_db.service.UserTaskService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;

@Controller
public class UserTaskController {
    @Autowired
    private UserTaskService userTaskService;
    @Autowired
    private UserService userService;
    @Autowired
    private TaskService taskService;

    @GetMapping("/usertasks/list")
    public String allUserTasks(Model model,
                               @RequestParam(defaultValue = "1") int page,
                               @RequestParam(required = false) String status,
                               @RequestParam(required = false) Integer id) {

        List<UserTaskModel> allUserTasks = userTaskService.findAll();
        List<UserModel> allUsers = userService.findAll();

        if (id != null) {
            UserTaskModel found = userTaskService.findById(id);
            allUserTasks = (found != null) ? List.of(found) : List.of();
        } else if (status != null && !status.trim().isEmpty()) {
            allUserTasks = userTaskService.findByStatus(status.trim());
        }

        int pageSize = 10;
        int totalItems = allUserTasks.size();
        int totalPages = (int) Math.ceil((double) totalItems / pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        int from = (page - 1) * pageSize;
        int to = Math.min(from + pageSize, totalItems);

        model.addAttribute("userTasks", allUserTasks.subList(from, to));
        model.addAttribute("users", allUsers);
        model.addAttribute("currentPage", page);
        model.addAttribute("totalPages", totalPages);
        model.addAttribute("searchStatus", status);
        model.addAttribute("searchId", id);

        return "userTaskList";
    }

    @PostMapping("/usertasks/add")
    public String addUserTask(@RequestParam int userId,
                              @RequestParam int taskId,
                              @RequestParam String status) {

        UserModel user = userService.findById(userId);
        TaskModel task = taskService.findById(taskId);

        if (user == null || task == null) return "redirect:/usertasks/list";

        UserTaskModel userTask = new UserTaskModel(0, user, task, status);
        userTaskService.addUserTask(userTask);
        return "redirect:/usertasks/list";
    }

    @PostMapping("/usertasks/update")
    public String updateUserTask(@RequestParam int id,
                                 @RequestParam int userId,
                                 @RequestParam int taskId,
                                 @RequestParam String status) {

        UserModel user = userService.findById(userId);
        TaskModel task = taskService.findById(taskId);
        if (user == null || task == null) return "redirect:/usertasks/list";

        UserTaskModel userTask = new UserTaskModel(id, user, task, status);
        userTaskService.updateUserTask(userTask);
        return "redirect:/usertasks/list";
    }

    @PostMapping("/usertasks/delete")
    public String deleteUserTask(@RequestParam int id) {
        userTaskService.deleteUserTask(id);
        return "redirect:/usertasks/list";
    }
}