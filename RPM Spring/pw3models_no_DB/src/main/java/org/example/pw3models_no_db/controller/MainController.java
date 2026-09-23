package org.example.pw3models_no_db.controller;

import jakarta.validation.Valid;
import org.example.pw3models_no_db.model.*;
import org.example.pw3models_no_db.service.*;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.validation.FieldError;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestParam;

import java.time.LocalDateTime;
import java.util.List;

@Controller
public class MainController {

    private final RoleService roleService;
    private final UserService userService;
    private final TaskService taskService;
    private final CategoryService categoryService;
    private final MessageService messageService;
    private final UserTaskService userTaskService;
    private final ResponseService responseService;
    private final PasswordEncoder passwordEncoder;

    public MainController(RoleService roleService,
                          UserService userService,
                          TaskService taskService,
                          CategoryService categoryService,
                          MessageService messageService,
                          UserTaskService userTaskService,
                          ResponseService responseService,
                          PasswordEncoder passwordEncoder) {
        this.roleService = roleService;
        this.userService = userService;
        this.taskService = taskService;
        this.categoryService = categoryService;
        this.messageService = messageService;
        this.userTaskService = userTaskService;
        this.responseService = responseService;
        this.passwordEncoder = passwordEncoder;
    }

    private UserModel currentUser() {
        Authentication auth = SecurityContextHolder.getContext().getAuthentication();
        if (auth == null || !(auth.getPrincipal() instanceof org.springframework.security.core.userdetails.UserDetails)) {
            return null;
        }
        String username = auth.getName();
        return userService.findAll().stream()
                .filter(u -> u.getUsername() != null && u.getUsername().equalsIgnoreCase(username))
                .findFirst()
                .orElse(null);
    }

    @GetMapping("/")
    public String home(@RequestParam(required = false) String search,
                       @RequestParam(required = false) Integer categoryId,
                       Model model) {
        List<TaskModel> allTasks = taskService.findAll();

        if (search != null && !search.trim().isEmpty()) {
            String s = search.trim().toLowerCase();
            allTasks = allTasks.stream()
                    .filter(t -> (t.getTitle() != null && t.getTitle().toLowerCase().contains(s))
                            || (t.getDescription() != null && t.getDescription().toLowerCase().contains(s)))
                    .toList();
        }
        if (categoryId != null) {
            allTasks = allTasks.stream()
                    .filter(t -> t.getCategory() != null && t.getCategory().getId() == categoryId)
                    .toList();
        }

        model.addAttribute("tasks", allTasks);
        model.addAttribute("categories", categoryService.findAll());
        model.addAttribute("search", search);
        model.addAttribute("searchCategoryId", categoryId);
        model.addAttribute("currentUser", currentUser());
        return "index";
    }

    @GetMapping("/admin")
    public String adminPanel(Model model) {
        model.addAttribute("currentUser", currentUser());
        return "admin";
    }

    @GetMapping("/login")
    public String login() {
        return "login";
    }

    @GetMapping("/access-denied")
    public String denied() {
        return "access-denied";
    }

    @GetMapping("/register")
    public String registerPage() {
        return "register";
    }

    @PostMapping("/register")
    public String register(@Valid UserModel user, BindingResult result) {

        if (result.hasErrors()) {
            return "redirect:/register?error=validation";
        }

        if (userService.findOneByUsername(user.getUsername()) != null) {
            return "redirect:/register?error=exists";
        }

        RoleModel role = roleService.findByRolNameExact("USER");
        if (role == null) {
            List<RoleModel> roles = roleService.findAll();
            role = roles.isEmpty() ? null : roles.get(0);
        }

        String encodedPassword = passwordEncoder.encode(user.getPassword());

        user.setPassword(encodedPassword);
        user.setRole(role);

        userService.addUser(user);

        return "redirect:/login?success";
    }

    @GetMapping("/profile")
    public String profile(Model model) {
        UserModel me = currentUser();
        if (me == null) return "redirect:/login";

        model.addAttribute("currentUser", me);

        List<TaskModel> myTasks = taskService.findAll().stream()
                .filter(t -> t.getUser() != null && t.getUser().getId() == me.getId())
                .toList();
        model.addAttribute("myTasks", myTasks);

        List<UserTaskModel> myUserTasks = userTaskService.findByUserId(me.getId());
        model.addAttribute("myUserTasks", myUserTasks);

        return "profile";
    }

    @GetMapping("/task/{id}")
    public String taskDetail(@PathVariable int id, Model model) {
        TaskModel task = taskService.findById(id);
        if (task == null) return "redirect:/";

        UserModel me = currentUser();
        boolean canManageTask = me != null
                && task.getUser() != null
                && (me.getId() == task.getUser().getId()
                    || (me.getRole() != null
                        && ("ADMIN".equalsIgnoreCase(me.getRole().getRolName())
                            || "MANAGER".equalsIgnoreCase(me.getRole().getRolName()))));

        model.addAttribute("task", task);
        model.addAttribute("responses", responseService.findByTaskId(id));
        model.addAttribute("currentUser", me);
        model.addAttribute("canManageTask", canManageTask);
        return "taskDetail";
    }

    @GetMapping("/tasks/create")
    public String createTaskPage(Model model) {
        model.addAttribute("categories", categoryService.findAll());
        model.addAttribute("currentUser", currentUser());
        return "create-task";
    }

    @PostMapping("/tasks/create")
    public String createTask(@RequestParam String title,
                             @RequestParam String description,
                             @RequestParam String deadline,
                             @RequestParam double price,
                             @RequestParam int categoryId) {
        UserModel me = currentUser();
        if (me == null) return "redirect:/login";

        CategoryModel category = categoryService.findById(categoryId);
        if (category == null) return "redirect:/tasks/create?error=category";

        try {
            TaskModel task = new TaskModel(0, title, description,
                    java.time.LocalDate.parse(deadline), price, me, category);
            task.setStatus("Открыто");
            taskService.addTask(task);
            return "redirect:/?created=true";
        } catch (Exception e) {
            return "redirect:/tasks/create?error=invalid";
        }
    }

    @PostMapping("/task/apply")
    public String applyTask(@RequestParam int taskId,
                            @RequestParam String message) {
        UserModel me = currentUser();
        if (me == null) return "redirect:/login";

        TaskModel task = taskService.findById(taskId);
        if (task == null) return "redirect:/";

        ResponseModel response = new ResponseModel(0, me, task, message, "ожидает");
        responseService.addResponse(response);

        boolean alreadyTaken = userTaskService.findByUserId(me.getId()).stream()
                .anyMatch(ut -> ut.getTask() != null && ut.getTask().getId() == taskId);

        if (!alreadyTaken) {
            UserTaskModel userTask = new UserTaskModel(0, me, task, "в работе");
            userTaskService.addUserTask(userTask);
        }

        return "redirect:/task/" + taskId + "?applied=true";
    }

    @PostMapping("/task/response/{id}/accept")
    public String acceptResponse(@PathVariable int id) {
        UserModel me = currentUser();
        if (me == null) return "redirect:/login";

        ResponseModel response = responseService.findById(id);
        if (response == null || response.getTask() == null) return "redirect:/";

        TaskModel task = response.getTask();
        boolean isOwner = task.getUser() != null && task.getUser().getId() == me.getId();
        boolean isModerator = me.getRole() != null &&
                ("ADMIN".equalsIgnoreCase(me.getRole().getRolName())
                        || "MANAGER".equalsIgnoreCase(me.getRole().getRolName()));
        if (!isOwner && !isModerator) return "redirect:/access-denied";

        response.setStatus("принят");
        responseService.updateResponse(response);

        for (ResponseModel other : responseService.findByTaskId(task.getId())) {
            if (other.getId() != id && !"принят".equalsIgnoreCase(other.getStatus())) {
                other.setStatus("отклонён");
                responseService.updateResponse(other);
            }
        }

        boolean alreadyTaken = userTaskService.findAll().stream()
                .anyMatch(ut -> ut.getTask() != null && ut.getTask().getId() == task.getId());
        if (!alreadyTaken) {
            userTaskService.addUserTask(new UserTaskModel(0, response.getUser(), task, "в работе"));
        }

        task.setStatus("В работе");
        taskService.updateTask(task);

        return "redirect:/task/" + task.getId() + "?accepted=true";
    }

    @PostMapping("/task/response/{id}/reject")
    public String rejectResponse(@PathVariable int id) {
        UserModel me = currentUser();
        if (me == null) return "redirect:/login";

        ResponseModel response = responseService.findById(id);
        if (response == null || response.getTask() == null) return "redirect:/";

        TaskModel task = response.getTask();
        boolean isOwner = task.getUser() != null && task.getUser().getId() == me.getId();
        boolean isModerator = me.getRole() != null &&
                ("ADMIN".equalsIgnoreCase(me.getRole().getRolName())
                        || "MANAGER".equalsIgnoreCase(me.getRole().getRolName()));
        if (!isOwner && !isModerator) return "redirect:/access-denied";

        response.setStatus("отклонён");
        responseService.updateResponse(response);

        return "redirect:/task/" + task.getId() + "?rejected=true";
    }

    @GetMapping("/messages")
    public String messages(Model model) {
        UserModel me = currentUser();
        if (me == null) return "redirect:/login";

        List<MessageModel> myMessages = messageService.findAll().stream()
                .filter(m -> (m.getSender() != null && m.getSender().getId() == me.getId())
                        || (m.getReceiver() != null && m.getReceiver().getId() == me.getId()))
                .sorted((a, b) -> {
                    if (a.getCreatedAt() == null) return 1;
                    if (b.getCreatedAt() == null) return -1;
                    return a.getCreatedAt().compareTo(b.getCreatedAt());
                })
                .toList();

        model.addAttribute("messages", myMessages);
        model.addAttribute("users", userService.findAll());
        model.addAttribute("currentUser", me);
        return "messages";
    }

    @PostMapping("/messages/send")
    public String sendMessage(@RequestParam int receiverId,
                              @RequestParam String text) {
        UserModel me = currentUser();
        if (me == null) return "redirect:/login";

        UserModel receiver = userService.findById(receiverId);
        if (receiver == null) return "redirect:/messages";

        MessageModel message = new MessageModel(0, me, receiver, text, LocalDateTime.now());
        messageService.addMessage(message);
        return "redirect:/messages?sent=true";
    }
}