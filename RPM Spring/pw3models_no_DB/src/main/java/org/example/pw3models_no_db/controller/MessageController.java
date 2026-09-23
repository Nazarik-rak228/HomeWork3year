package org.example.pw3models_no_db.controller;

import org.example.pw3models_no_db.model.MessageModel;
import org.example.pw3models_no_db.model.UserModel;
import org.example.pw3models_no_db.service.MessageService;
import org.example.pw3models_no_db.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.time.LocalDateTime;
import java.util.List;

@Controller
public class MessageController {
    @Autowired
    private MessageService messageService;
    @Autowired
    private UserService userService;

    @GetMapping("/messages/list")
    public String allMessages(Model model,
                              @RequestParam(defaultValue = "1") int page,
                              @RequestParam(required = false) Integer senderId,
                              @RequestParam(required = false) Integer id) {

        List<MessageModel> allMessages = messageService.findAll();
        List<UserModel> users = userService.findAll();

        if (id != null) {
            MessageModel found = messageService.findById(id);
            allMessages = (found != null) ? List.of(found) : List.of();
        } else if (senderId != null) {
            allMessages = messageService.findBySenderId(senderId);
        }

        int pageSize = 10;
        int totalItems = allMessages.size();
        int totalPages = (int) Math.ceil((double) totalItems / pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        int from = (page - 1) * pageSize;
        int to = Math.min(from + pageSize, totalItems);

        model.addAttribute("messages", allMessages.subList(from, to));
        model.addAttribute("currentPage", page);
        model.addAttribute("totalPages", totalPages);
        model.addAttribute("searchSenderId", senderId);
        model.addAttribute("searchId", id);
        model.addAttribute("users", users);
        return "messageList";
    }

    @PostMapping("/messages/add")
    public String addMessage(@RequestParam int senderId,
                             @RequestParam int receiverId,
                             @RequestParam String text) {

        UserModel sender = userService.findById(senderId);
        UserModel receiver = userService.findById(receiverId);

        if (sender == null || receiver == null) return "redirect:/messages/list";

        MessageModel message = new MessageModel(0, sender, receiver, text, LocalDateTime.now());
        messageService.addMessage(message);
        return "redirect:/messages/list";
    }

    @PostMapping("/messages/update")
    public String updateMessage(@RequestParam int id,
                                @RequestParam int senderId,
                                @RequestParam int receiverId,
                                @RequestParam String text) {

        UserModel sender = userService.findById(senderId);
        UserModel receiver = userService.findById(receiverId);
        if (sender == null || receiver == null) return "redirect:/messages/list";

        MessageModel message = new MessageModel(id, sender, receiver, text, LocalDateTime.now());
        messageService.updateMessage(message);
        return "redirect:/messages/list";
    }

    @PostMapping("/messages/delete")
    public String deleteMessage(@RequestParam int id) {
        messageService.deleteMessage(id);
        return "redirect:/messages/list";
    }
}